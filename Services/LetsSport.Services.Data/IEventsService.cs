namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;
    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Events;

    public interface IEventsService
    {
        Task CreateAsync(EventCreateInputModel inputModel, string userId);

        public EventDetailsViewModel GetEvent(int id);

        public EventEditViewModel GetDetailsForEdit(int id);

        void UpdateEvent(EventEditViewModel viewModel);

        EventsAllDetailsViewModel GetAll();

        int GetIdByChatRoomId(string chatRoomId);

        bool IsUserJoined(string username, int eventId);

        Task AddUserAsync(int eventId, string userId);

        Task RemoveUserAsync(int eventId, string userId);
    }
}
