namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Contacts;

    public interface IContactsService
    {
        Task<int> FileContactFormAsync(ContactIndexViewModel inputModel);

        ContactTankYouViewModel SayThankYou(string senderName);

        Task<int> GetCountAsync();

        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<T> GetByIdAsync<T>(int id);
    }
}
