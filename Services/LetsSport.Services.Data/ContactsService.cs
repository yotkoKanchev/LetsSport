namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.Contacts;

    public class ContactsService : IContactsService
    {
        private readonly IRepository<ContactForm> contactFormsRepository;
        private readonly IEmailSender emailSender;

        public ContactsService(IRepository<ContactForm> contactFormsRepository, IEmailSender emailSender)
        {
            this.contactFormsRepository = contactFormsRepository;
            this.emailSender = emailSender;
        }

        public async Task FileContactForm(ContactIndexViewModel inputModel)
        {
            var contactFormEntry = inputModel.To<ContactForm>();
            await this.contactFormsRepository.AddAsync(contactFormEntry);
            await this.contactFormsRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                inputModel.Email,
                inputModel.Name,
                GlobalConstants.SystemEmail,
                EmailHtmlMessages.GetContactFormContentHtml(inputModel.Name, inputModel.Title, inputModel.Content));
        }

        public ContactTankYouViewModel SayThankYou(string senderName)
        {
            return new ContactTankYouViewModel
            {
                SenderName = senderName,
            };
        }
    }
}
