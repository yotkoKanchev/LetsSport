namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.EntityFrameworkCore;

    public class ContactsService : IContactsService
    {
        private readonly IRepository<ContactForm> contactFormsRepository;
        private readonly IEmailSender emailSender;

        public ContactsService(IRepository<ContactForm> contactFormsRepository, IEmailSender emailSender)
        {
            this.contactFormsRepository = contactFormsRepository;
            this.emailSender = emailSender;
        }

        public async Task<int> FileContactFormAsync(ContactIndexViewModel inputModel)
        {
            var contactFormEntry = new ContactForm
            {
                Email = inputModel.Email,
                Content = inputModel.Content,
                Name = inputModel.Name,
                Title = inputModel.Title,
            };

            await this.contactFormsRepository.AddAsync(contactFormEntry);
            await this.contactFormsRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                inputModel.Email,
                inputModel.Name,
                GlobalConstants.SystemEmail,
                EmailHtmlMessages.GetContactFormContentHtml(inputModel.Name, inputModel.Title, inputModel.Content));

            return contactFormEntry.Id;
        }

        public ContactTankYouViewModel SayThankYou(string senderName)
        {
            return new ContactTankYouViewModel
            {
                SenderName = senderName,
            };
        }

        public async Task<int> GetCountAsync()
        {
            return await this.contactFormsRepository.All().CountAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.contactFormsRepository.All()
                .OrderByDescending(r => r.CreatedOn)
                .ThenByDescending(r => r.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            var form = this.contactFormsRepository.All()
                .Where(r => r.Id == id)
                .To<T>();

            return await form.FirstOrDefaultAsync();
        }
    }
}
