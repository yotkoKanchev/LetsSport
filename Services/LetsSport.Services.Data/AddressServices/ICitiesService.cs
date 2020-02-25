namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICitiesService
    {
        Task CreateCityAsync(string cityName, int countryId);

        Task<int> GetCityIdAsync(string cityName, string country);

        Task<IEnumerable<string>> GetCitiesAsync();

        bool IsCityExists(string cityName, string countryName);
    }
}
