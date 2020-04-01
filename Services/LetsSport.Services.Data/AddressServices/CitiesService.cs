namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.Administration.Cities;
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
            var countries = this.citiesRepository
                .All()
                .OrderBy(c => c.Country.Id)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });

            return countries;
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

        public IQueryable<City> GetAll()
        {
            IQueryable<City> cities = this.citiesRepository
                .All()
                .OrderBy(c => c.CountryId)
                .ThenBy(c => c.Name);

            return cities;
        }

        public CitiesIndexViewModel FilterCities(int? country, int isDeleted)
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
                 .Select(c => new CityInfoViewModel
                 {
                     Id = c.Id,
                     Name = c.Name,
                     CountryId = c.CountryId,
                     CountryName = c.Country.Name,
                     CreatedOn = c.CreatedOn,
                     DeletedOn = c.DeletedOn,
                     IsDeleted = c.IsDeleted,
                     ModifiedOn = c.ModifiedOn,
                 })
                 .ToList();

                var viewModel = new CitiesIndexViewModel
                {
                    Cities = cities,
                    Filter = new CitiesFilterBarViewModel
                    {
                        Countries = this.countriesService.GetAll(),
                        Country = country,
                        IsDeleted = isDeleted,
                    },
                };

                return viewModel;
        }
    }
}
