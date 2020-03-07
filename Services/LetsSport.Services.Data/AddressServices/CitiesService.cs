namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Services.Data.Common;

    public class CitiesService : ICitiesService
    {
        private readonly IRepository<City> citiesRepository;
        private readonly ICountriesService countriesService;
        private readonly ILocationLocator locator;

        public CitiesService(
            IRepository<City> citiesRepository,
            ICountriesService countriesService,
            ILocationLocator locator)
        {
            this.citiesRepository = citiesRepository;
            this.countriesService = countriesService;
            this.locator = locator;
        }

        public async Task CreateCityAsync(string cityName, int countryId)
        {
            var city = new City
            {
                Name = cityName,
                CountryId = countryId,
            };

            await this.citiesRepository.AddAsync(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task<int> GetCityIdAsync(string cityName, string country)
        {
            var countryId = this.countriesService.GetCountryId(country);

            if (!this.IsCityExists(cityName, countryId))
            {
                await this.CreateCityAsync(cityName, countryId);
            }

            return this.citiesRepository
                .AllAsNoTracking()
                .Where(c => c.Name == cityName && c.CountryId == countryId)
                .Select(c => c.Id)
                .First();
        }

        public async Task<IEnumerable<string>> GetCitiesAsync()
        {
            var currentLocation = this.locator.GetLocationInfo();
            var cityName = currentLocation.City;
            var countryName = currentLocation.Country;
            int countryId = this.countriesService.GetCountryId(countryName);

            if (!this.IsCityExists(cityName, countryId))
            {
                await this.CreateCityAsync(cityName, countryId);
            }

            var cities = this.citiesRepository
                .AllAsNoTracking()
                .Where(c => c.Country.Id == countryId)
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToList();

            return cities;
        }

        public bool IsCityExists(string cityName, int countryId) =>
            this.citiesRepository.AllAsNoTracking().Any(c => c.Name == cityName && c.Country.Id == countryId);
    }
}