namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICitiesService
    {
        Task CreateCityAsync(string cityName, int countryId);

        Task<int> GetCityIdAsync(string cityName, string country);

        Task<IEnumerable<string>> GetCitiesAsync(string cityName, string countryName);

        Task<IEnumerable<string>> GetCitiesWhitEventsAsync(string cityName, string countryName);

        bool IsCityExists(string cityName, int countryId);
    }
}
