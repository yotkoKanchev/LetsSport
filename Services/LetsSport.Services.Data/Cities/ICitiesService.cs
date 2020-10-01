namespace LetsSport.Services.Data.Cities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICitiesService
    {
        Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId);

        Task<IEnumerable<SelectListItem>> GetAllWithArenasInCountryAsync(int countryId);

        Task<IEnumerable<SelectListItem>> GetAllWithEventsInCountryAsync(int countryId);

        Task<T> GetByIdAsync<T>(int cityId);

        Task<int> GetIdAsync(string cityName, int countryId);

        Task<string> GetNameByIdAsync(int cityId);

        Task<bool> IsExistsAsync((string CityName, string CountryName) location);

        // Admin
        Task<IEnumerable<T>> GetAllByCountryIdAsync<T>(int countryId, int? take = null, int skip = 0);

        Task CreateAsync((string CityName, string CountryName) location);

        Task UpdateAsync(int id, string name, int countryId, bool isDeleted);

        Task ArchiveByIdAsync(int id);

        Task DeleteByIdAsync(int id);

        Task<IndexViewModel> FilterAsync(int countryId, int deletionStatus, int? take = null, int skip = 0);

        Task<int> GetCountInCountryAsync(int countryId);
    }
}
