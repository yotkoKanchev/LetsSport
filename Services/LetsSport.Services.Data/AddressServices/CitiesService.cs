namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Data.Models.EventModels;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CitiesService : ICitiesService
    {
        private readonly IRepository<City> citiesRepository;
        private readonly IRepository<Event> eventsRepository;
        private readonly ICountriesService countriesService;

        public CitiesService(
            IRepository<City> citiesRepository,
            IRepository<Event> eventsRepository,
            ICountriesService countriesService)
        {
            this.citiesRepository = citiesRepository;
            this.eventsRepository = eventsRepository;
            this.countriesService = countriesService;
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
                .FirstOrDefault();
        }

        public async Task<IEnumerable<SelectListItem>> GetCitiesAsync((string City, string Country) location)
        {
            int countryId = this.countriesService.GetCountryId(location.Country);
            string cityName = location.City;

            if (!this.IsCityExists(cityName, countryId))
            {
                await this.CreateCityAsync(cityName, countryId);
            }

            var resultList = this.GetCitiesSelectList(countryId);

            return resultList;
        }

        public IEnumerable<SelectListItem> GetCitiesSelectList(int countryId)
        {
            var cities = this.citiesRepository
                .All()
                .Where(c => c.Country.Id == countryId)
                .OrderBy(c => c.Name);

            var resultList = new List<SelectListItem>();

            foreach (var city in cities)
            {
                resultList.Add(new SelectListItem { Value = city.Id.ToString(), Text = city.Name });
            }

            return resultList;
        }

        public async Task<IEnumerable<string>> GetCitiesWhitEventsAsync(string currentCity, string currentCountry)
        {
            int countryId = this.countriesService.GetCountryId(currentCountry);

            if (!this.IsCityExists(currentCity, countryId))
            {
                await this.CreateCityAsync(currentCity, countryId);
            }

            var cities = this.eventsRepository
                .All()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Select(c => c.Arena.Address.City.Name)
                .OrderBy(c => c)
                .ToHashSet();

            return cities;
        }

        public bool IsCityExists(string cityName, int countryId) =>
            this.citiesRepository.AllAsNoTracking().Any(c => c.Name == cityName && c.Country.Id == countryId);
    }
}
