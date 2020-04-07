namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Messages;

    public interface IMessagesService
    {
        Task CreateAsync(string messageText, string userId, int eventId);

        Task AddInitialMessageAsync(string userId, int eventId);

        Task<IEnumerable<MessageDetailsViewModel>> GetAllByEventIdAsync(int id);
    }
}
