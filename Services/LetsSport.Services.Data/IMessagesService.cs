namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Messages;

    public interface IMessagesService
    {
        Task<string> CreateAsync(string content, string userId, int eventId);

        Task<IEnumerable<MessageDetailsViewModel>> GetAllByEventIdAsync(int id);

        Task<MessageDetailsViewModel> GetDetailsById(string id);
    }
}
