namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CitiesService : ICitiesService
    {
        private readonly IRepository<City> citiesRepository;
        private readonly IRepository<Event> eventsRepository;
        private readonly IRepository<Arena> arenasRepository;
        private readonly ICountriesService countriesService;

        public CitiesService(
            IRepository<City> citiesRepository,
            IRepository<Event> eventsRepository,
            IRepository<Arena> arenasRepository,
            ICountriesService countriesService)
        {
            this.citiesRepository = citiesRepository;
            this.eventsRepository = eventsRepository;
            this.arenasRepository = arenasRepository;
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

        public async Task<int> GetCityIdAsync((string CityName, string Country) location)
        {
            var countryId = this.countriesService.GetCountryId(location.Country);

            if (!this.IsCityExists(location.CityName, countryId))
            {
                await this.CreateCityAsync(location.CityName, countryId);
            }

            return this.citiesRepository
                .AllAsNoTracking()
                .Where(c => c.Name == location.CityName && c.CountryId == countryId)
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

            var resultList = this.GetCitiesSelectList(location.Country);

            return resultList;
        }

        public IEnumerable<SelectListItem> GetCitiesSelectList(string countryName)
        {
            var cities = this.citiesRepository
                .All()
                .Where(c => c.Country.Name == countryName)
                .OrderBy(c => c.Name);

            var resultList = new List<SelectListItem>();

            foreach (var city in cities)
            {
                resultList.Add(new SelectListItem { Value = city.Id.ToString(), Text = city.Name });
            }

            return resultList;
        }

        public IEnumerable<SelectListItem> GetCitiesWithEventsAsync(string country)
        {
            return this.citiesRepository
                 .All()
                 .Where(c => c.Country.Name == country)
                 .Where(c => c.Events.Any())
                 .OrderBy(c => c)
                 .Select(c => new SelectListItem
                 {
                     Text = c.Name,
                     Value = c.Id.ToString(),
                 });
        }

        public IList<SelectListItem> GetCitiesWithArenas(string country)
        {
            int countryId = this.countriesService.GetCountryId(country);

            var cities = this.citiesRepository
                .All()
                .Where(c => c.Country.Name == country)
                .Where(c => c.Arenas.Any())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                })
                .ToList();

            return cities;
        }

        public bool IsCityExists(string cityName, int countryId) =>
            this.citiesRepository.AllAsNoTracking().Any(c => c.Name == cityName && c.Country.Id == countryId);

        public string GetLocationByCityId(int cityId)
        {
            return this.citiesRepository
                .All()
                .Where(c => c.Id == cityId)
                .Select(c => c.Name + ", " + c.Country.Name)
                .FirstOrDefault();
        }

        public int GetCityIdByArenaId(int arenaId)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.Id == arenaId)
                .Select(a => a.CityId)
                .FirstOrDefault();
        }

        public string GetCityNameById(int cityId)
        {
            return this.citiesRepository
                .All()
                .Where(c => c.Id == cityId)
                .Select(c => c.Name)
                .FirstOrDefault();
        }
    }
}
