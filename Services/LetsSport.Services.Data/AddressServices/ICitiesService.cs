namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICitiesService
    {
        Task CreateCityAsync(string cityName, int countryId);

        IEnumerable<SelectListItem> GetCities(int countryId);

        Task<int> GetCityIdAsync(string cityName, string country);

        // Task<IEnumerable<string>> GetCitiesAsync(string cityName, string countryName);
        Task<IEnumerable<SelectListItem>> GetCitiesAsync((string City, string Country) location);

        Task<IEnumerable<string>> GetCitiesWhitEventsAsync(string cityName, string countryName);

        bool IsCityExists(string cityName, int countryId);
    }
}
