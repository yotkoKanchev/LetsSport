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
        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        EventEditViewModel GetDetailsForEdit(int id, (string City, string Country) location);

        EventDetailsViewModel GetDetails(int id, string userId);

        Task UpdateEvent(EventEditViewModel viewModel);

        Task CancelEvent(int id, string userEmail, string username);

        Task<int> InviteUsersToEvent(int id, string email, string userName);

        Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, string country, string userId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, ApplicationUser user);

        Task<IEnumerable<T>> GetUpcomingEvents<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetAllAdministratingEventsByUserId<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetNotParticipatingEventsInCity<T>(string userId, (string City, string Country) location, int? count = null);

        Task<IEnumerable<T>> GetCanceledEvents<T>(string userId, string country, int? count = null);

        Task<IEnumerable<T>> GetAllInCity<T>((string City, string Country) location, int? count = null);

        bool IsUserJoined(string userId, int eventId);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId, string country);
    }
}
