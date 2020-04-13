namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Admin.Events;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public interface IEventsService
    {
        Task<IEnumerable<T>> GetAllUpcomingByUserIdAsync<T>(string userId, int? take = null, int skip = 0);

        Task<IEnumerable<T>> GetAllAdministratingByUserIdAsync<T>(string userId, int? take = null, int skip = 0);

        Task<IEnumerable<T>> GetNotParticipatingInCityAsync<T>(string userId, int cityId, int? take = null, int skip = 0);

        Task<int> GetNotParticipatingCount(string userId, int cityId);

        Task<IEnumerable<T>> GetAdminAllCanceledAsync<T>(string userId, int? take = null, int skip = 0);

        Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId, int? take = null, int skip = 0);

        Task SetPassedStatusAsync(int countryId);

        Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username);

        Task<EventEditViewModel> GetDetailsForEditAsync(int eventId);

        Task UpdateAsync(EventEditViewModel viewModel);

        Task<EventDetailsViewModel> GetDetailsAsync(int id, string userId);

        Task AddUserAsync(int eventId, string userId, string userEmail, string username);

        Task RemoveUserAsync(int eventId, string userId, string username, string email);

        Task CancelEventAsync(int id, string userEmail, string username);

        Task ChangeStatus(int eventId, ArenaRequestStatus approved);

        Task<int> InviteUsersToEventAsync(int id, string email, string userName);

        Task<HomeEventsListViewModel> FilterEventsAsync(
            int? city, int? sport, DateTime from, DateTime to, int countryId, string userId, int? take = null, int skip = 0);

        Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId);

        bool IsUserJoined(string userId, int eventId);

        Task SetSentRequestStatus(int id);

        // Admin
        Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId, int? take = null, int skip = 0);

        Task<IndexViewModel> AdminFilterAsync(int countryId, int? cityId, int? sportId, int? isDeleted, int? take = null, int skip = 0);

        Task<T> GetEventByIdAsync<T>(int value);

        Task AdminUpdateAsync(EditViewModel inputModel);

        Task DeleteByIdAsync(int id);

        Task<int> GetCountInCountryAsync(int countryId);

        Task<int> GetCountInCityAsync(int cityId);
    }
}
