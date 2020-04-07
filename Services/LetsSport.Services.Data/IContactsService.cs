namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Contacts;

    public interface IContactsService
    {
        Task FileContactFormAsync(ContactIndexViewModel inputModel);

        ContactTankYouViewModel SayThankYou(string senderName);
    }
}
