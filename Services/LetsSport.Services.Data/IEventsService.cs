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
        Task<IEnumerable<T>> GetAllUpcomingByUserIdAsync<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetAllAdministratingByUserIdAsync<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetNotParticipatingInCityAsync<T>(string userId, int cityId, int? count = null);

        Task<IEnumerable<T>> GetAdminAllCanceledAsync<T>(string userId, int? count = null);

        Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId, int? count = null);

        Task SetPassedStatusAsync(int countryId);

        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        Task<EventEditViewModel> GetDetailsForEditAsync(int eventId);

        Task UpdateAsync(EventEditViewModel viewModel);

        Task<EventDetailsViewModel> GetDetailsAsync(int id, string userId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, string userId, string username, string email);

        Task CancelEventAsync(int id, string userEmail, string username);

        Task<int> InviteUsersToEventAsync(int id, string email, string userName);

        Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, int countryId, string userId);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId);

        bool IsUserJoined(string userId, int eventId);

        // Admin
        Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId, int? take = null, int skip = 0);

        Task<IndexViewModel> AdminFilterAsync(int countryId, int? cityId, int? sportId, int? isDeleted, int? take = null, int skip = 0);

        Task<T> GetEventByIdAsync<T>(int value);

        Task AdminUpdateAsync(EditViewModel inputModel);

        Task DeleteByIdAsync(int id);

        Task<int> GetCountInCountryAsync(int countryId);
    }
}
