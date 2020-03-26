namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<IEnumerable<T>> GetAllInCity<T>((string City, string Country) location, int? count = null);

        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        EventDetailsViewModel GetDetailsWithChatRoom(int id, string userId = null, string username = null);

        EventEditViewModel GetDetailsForEdit(int id, (string City, string Country) location);

        Task UpdateEvent(EventEditViewModel viewModel);

        bool IsUserJoined(string username, int eventId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, ApplicationUser user);

        Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, string country);

        Task<HomeIndexLoggedEventsListViewModel> FilterEventsLoggedAsync(int city, int sport, DateTime from, DateTime to, string userId, string country);

        Task<IEnumerable<T>> GetAllAdministratingEventsByUserId<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetUpcomingEvents<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetNotParticipatingEventsInCity<T>(string userId, (string City, string Country) location, int? count = null);

        Task<IEnumerable<T>> GetCanceledEvents<T>(string userId, string country, int? count = null);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId, string country);

        Task CancelEvent(int id, string userEmail, string username);

        Task<int> InviteUsersToEvent(int id, string email, string userName);
    }
}
