namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string city, string country);

        public EventDetailsViewModel GetEvent(int id);

        public EventEditViewModel GetDetailsForEdit(int id, string city, string country);

        Task UpdateEvent(EventEditViewModel viewModel);

        Task<EventsAllDetailsViewModel> GetAll(string cityName, string countryName);

        int GetIdByChatRoomId(string chatRoomId);

        bool IsUserJoined(string username, int eventId);

        Task AddUserAsync(int eventId, string userId);

        Task RemoveUserAsync(int eventId, string userId);

        Task<EventsAllDetailsViewModel> FilterEventsAsync(EventsFilterInputModel inputModel, string cityName, string countryName);
    }
}
