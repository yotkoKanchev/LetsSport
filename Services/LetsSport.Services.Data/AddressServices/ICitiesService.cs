namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICitiesService
    {
        IEnumerable<T> GetAll<T>();

        IEnumerable<SelectListItem> GetAllAsSelectList();

        Task CreateCityAsync(string cityName, int countryId);

        IEnumerable<SelectListItem> GetCitiesInCountryById(int countryId);

        Task<int> GetCityIdAsync((string CityName, string Country) location);

        Task<IEnumerable<SelectListItem>> GetCitiesAsync((string City, string Country) location);

        IEnumerable<SelectListItem> GetCitiesWithEventsAsync(string country);

        bool IsCityExists(string cityName, int countryId);

        IList<SelectListItem> GetCitiesWithArenas(string country);

        string GetLocationByCityId(int cityId);

        int GetCityIdByArenaId(int arenaId);

        string GetCityNameById(int cityId);

        IndexViewModel FilterCities(int? country, int status);

        T GetCityById<T>(int cityId);

        Task UpdateCityAsync(int id, string name, int countryId, bool isDeleted);

        Task DeleteById(int id);
    }
}
