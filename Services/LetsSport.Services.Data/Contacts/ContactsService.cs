namespace LetsSport.Services.Data.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.GlobalConstants;

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
                SystemEmail,
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

        public async Task<T> GetByIdAsync<T>(int id)
        {
            var form = this.contactFormsRepository.All()
                .Where(r => r.Id == id)
                .To<T>();

            return await form.FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await this.contactFormsRepository.All()
                .Where(cf => cf.IsReplyed == false)
                .CountAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.contactFormsRepository.All()
                .OrderBy(r => r.CreatedOn)
                .Where(r => r.IsReplyed == false)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task ReplyAsync(int id, string replyContent, string recipientEmail)
        {
            await this.emailSender.SendEmailAsync(
                        recipientEmail,
                        EmailSubjectConstants.ContactFormReply,
                        EmailHtmlMessages.GetContactFormReplyHtml(replyContent));

            await this.SetReplyedAsync(id, replyContent);
        }

        public async Task IgnoreAsync(int id)
        {
            await this.SetReplyedAsync(id, "IGNORED");
        }

        private async Task SetReplyedAsync(int id, string replyContent)
        {
            var form = this.contactFormsRepository.All()
                .FirstOrDefault(r => r.Id == id);

            form.ReplyContent = replyContent;
            form.IsReplyed = true;
            form.ReplyedOn = DateTime.UtcNow;

            this.contactFormsRepository.Update(form);
            await this.contactFormsRepository.SaveChangesAsync();
        }
    }
}
