namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICitiesService
    {
        Task CreateCityAsync(string cityName, int countryId);

        IEnumerable<SelectListItem> GetCitiesSelectList(int countryId);

        Task<int> GetCityIdAsync(string cityName, string country);

        // Task<IEnumerable<string>> GetCitiesAsync(string cityName, string countryName);
        Task<IEnumerable<SelectListItem>> GetCitiesAsync((string City, string Country) location);

        IEnumerable<string> GetCitiesWithEventsAsync(string country);

        bool IsCityExists(string cityName, int countryId);

        IList<SelectListItem> GetCitiesWithArenasAsync(string country);

        string GetLocationByCityId(int city);
    }
}
