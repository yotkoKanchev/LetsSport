namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Contacts;

    public interface IContactsService
    {
        Task FileContactForm(ContactIndexViewModel inputModel, string ip);

        ContactTankYouViewModel SayThankYou(string senderName);
    }
}
