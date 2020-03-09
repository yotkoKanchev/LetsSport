namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId);

        public EventDetailsViewModel GetEvent(int id);

        public EventEditViewModel GetDetailsForEdit(int id);

        Task UpdateEvent(EventEditViewModel viewModel);

        Task<EventsAllDetailsViewModel> GetAll(string ip);

        int GetIdByChatRoomId(string chatRoomId);

        bool IsUserJoined(string username, int eventId);

        Task AddUserAsync(int eventId, string userId);

        Task RemoveUserAsync(int eventId, string userId);

        Task<EventsAllDetailsViewModel> FilterEventsAsync(EventsFilterInputModel inputModel, string ip);
    }
}
