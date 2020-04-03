namespace LetsSport.Services.Data.AddressServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CitiesService : ICitiesService
    {
        private readonly IRepository<City> citiesRepository;
        private readonly IRepository<Arena> arenasRepository;
        private readonly ICountriesService countriesService;

        public CitiesService(
            IRepository<City> citiesRepository,
            IRepository<Arena> arenasRepository,
            ICountriesService countriesService)
        {
            this.citiesRepository = citiesRepository;
            this.arenasRepository = arenasRepository;
            this.countriesService = countriesService;
        }

        public IEnumerable<SelectListItem> GetAllAsSelectList()
        {
            var cities = this.citiesRepository
                .All()
                .OrderBy(c => c.Country.Id)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });

            return cities;
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

            var resultList = this.GetCitiesInCountryById(countryId);

            return resultList;
        }

        public IEnumerable<SelectListItem> GetCitiesInCountryById(int countryId)
        {
            var cities = this.citiesRepository
                .All()
                .Where(c => c.Country.Id == countryId)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });

            return cities;
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
            return this.GetCityAsIQueriable(cityId)
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
            return this.GetCityAsIQueriable(cityId)
                .Select(c => c.Name)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return this.citiesRepository
                .All()
                .OrderBy(c => c.CountryId)
                .ThenBy(c => c.Name)
                .To<T>()
                .ToList();
        }

        public IndexViewModel FilterCities(int? country, int isDeleted)
        {
            var query = this.citiesRepository
                .All();

            if (country != 0)
            {
                query = query
                    .Where(c => c.CountryId == country);
            }

            if (isDeleted != 0)
            {
                if (isDeleted == 1)
                {
                    query = query
                        .Where(c => c.IsDeleted == false);
                }
                else if (isDeleted == 2)
                {
                    query = query
                        .Where(c => c.IsDeleted == true);
                }
            }

            var cities = query
                 .OrderBy(c => c.Country.Name)
                 .ThenBy(c => c.Name)
                 .To<InfoViewModel>()
                 .ToList();

            var viewModel = new IndexViewModel
            {
                Cities = cities,
                Filter = new FilterBarViewModel
                {
                    Countries = this.countriesService.GetAll(),
                    Country = country,
                    IsDeleted = isDeleted,
                },
            };

            return viewModel;
        }

        public T GetCityById<T>(int cityId)
        {
            return this.GetCityAsIQueriable(cityId)
                .To<T>()
                .FirstOrDefault();
        }

        public async Task UpdateCityAsync(int id, string name, int countryId, bool isDeleted)
        {
            var city = this.GetCityAsIQueriable(id).FirstOrDefault();
            city.Name = name;
            city.CountryId = countryId;
            city.IsDeleted = isDeleted;

            this.citiesRepository.Update(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var city = this.GetCityAsIQueriable(id).FirstOrDefault();
            this.citiesRepository.Delete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        private IQueryable<City> GetCityAsIQueriable(int cityId)
        {
            var city = this.citiesRepository
                .All()
                .Where(c => c.Id == cityId);

            if (city == null)
            {
                throw new ArgumentException($"City with ID: {cityId} does not exists!");
            }

            return city;
        }
    }
}
