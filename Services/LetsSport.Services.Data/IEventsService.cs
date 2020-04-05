namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Admin.Events;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        Task<EventEditViewModel> GetDetailsForEditAsync(int id, (string City, string Country) location);

        EventDetailsViewModel GetDetails(int id, string userId);

        Task UpdateEvent(EventEditViewModel viewModel);

        Task CancelEvent(int id, string userEmail, string username);

        Task<int> InviteUsersToEvent(int id, string email, string userName);

        Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, int countryId, string userId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, string userId, string username, string email);

        Task<IEnumerable<T>> GetUpcomingEvents<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetAllAdministratingEventsByUserId<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetNotParticipatingEventsInCity<T>(string userId, int cityId, int? count = null);

        Task<IEnumerable<T>> GetCanceledEvents<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetAllInCity<T>(int cityId, int? count = null);

        bool IsUserJoined(string userId, int eventId);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId);

        IEnumerable<T> GetAllInCountry<T>(int countryId);

        Task<IndexViewModel> FilterEventsAsync(int countryId, int? city, int? sport);

        T GetEventById<T>(int value);

        Task AdminUpdateEventAsync(EditViewModel inputModel);

        Task DeleteById(int id);

        Task SetPassedStatusOnPassedEvents(int countryId);
    }
}
