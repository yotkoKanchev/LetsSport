namespace LetsSport.Services.Data.AddressServices
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

        T GetById<T>(int cityId);

        Task<int> GetIdAsync(string cityName, int countryId);

        string GetNameById(int cityId);

        bool IsExists(string cityName, int countryId);

        // Admin
        Task<IEnumerable<T>> GetAllByCountryIdAsync<T>(int countryId);

        Task CreateAsync(string cityName, int countryId);

        Task UpdateAsync(int id, string name, int countryId, bool isDeleted);

        Task DeleteById(int id);

        Task<IndexViewModel> FilterAsync(int countryId, int isDeleted);

        string GetLocationByCityId(int cityId);
    }
}
