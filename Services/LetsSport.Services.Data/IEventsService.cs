namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<IEnumerable<T>> GetAll<T>(string country, int? count = null);

        HashSet<string> GetAllSportsInCurrentCountry(string currentCountry);

        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        public EventDetailsViewModel GetDetailsWithChatRoom(int id);

        public EventEditViewModel GetDetailsForEdit(int id, (string City, string Country) location);

        Task UpdateEvent(EventEditViewModel viewModel);

        bool IsUserJoined(string username, int eventId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, string userId, string userEmail, string username);

        Task<HomeEventsListViewModel> FilterEventsAsync(string city, string sport, DateTime from, DateTime to, string country);

        Task<HomeIndexLoggedEventsListViewModel> FilterEventsLoggedAsync(string city, string sport, DateTime from, DateTime to, string userId, string country);

        Task<IEnumerable<T>> GetAllAdministratingEventsByUserId<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetUpcomingEvents<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetNotParticipatingEvents<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetCanceledEvents<T>(string userId, string country, int? count = null);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId, string country);

        Task CancelEvent(int id, string userEmail, string username);
    }
}
