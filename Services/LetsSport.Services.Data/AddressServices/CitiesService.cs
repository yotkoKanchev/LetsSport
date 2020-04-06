namespace LetsSport.Services.Data.AddressServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class CitiesService : ICitiesService
    {
        private readonly IRepository<City> citiesRepository;
        private readonly ICountriesService countriesService;

        public CitiesService(IRepository<City> citiesRepository, ICountriesService countriesService)
        {
            this.citiesRepository = citiesRepository;
            this.countriesService = countriesService;
        }

        public async Task<int> GetIdAsync(string cityName, int countryId)
        {
            return await this.GetAllInCountryAsIQueryable(countryId)
                .Where(c => c.Name == cityName)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId)
        {
            var cities = await this.GetAllInCountryAsIQueryable(countryId)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
                .ToListAsync();

            return cities;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllWithEventsInCountryAsync(int countryId)
        {
            return await this.GetAllInCountryAsIQueryable(countryId)
                 .Where(c => c.Events.Any())
                 .OrderBy(c => c)
                 .Select(c => new SelectListItem
                 {
                     Text = c.Name,
                     Value = c.Id.ToString(),
                 })
                 .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllWithArenasInCountryAsync(int countryId)
        {
            var cities = await this.GetAllInCountryAsIQueryable(countryId)
                .Where(c => c.Arenas.Any())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                })
                .ToListAsync();

            return cities;
        }

        public bool IsExists(string cityName, int countryId) =>
            this.citiesRepository.All()
            .Any(c => c.Name == cityName && c.Country.Id == countryId);

        // TODO remove this dummy method
        public string GetLocationByCityId(int cityId)
        {
            return this.GetAsIQueriable(cityId)
                .Select(c => c.Name + ", " + c.Country.Name)
                .FirstOrDefault();
        }

        public string GetNameById(int cityId)
        {
            return this.GetAsIQueriable(cityId)
                .Select(c => c.Name)
                .FirstOrDefault();
        }

        // Admin
        public async Task<IEnumerable<T>> GetAllByCountryIdAsync<T>(int countryId, int? take = null, int skip = 0)
        {
            var query = this.GetAllInCountryAsIQueryable(countryId)
                .OrderBy(c => c.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query
              .To<T>()
              .ToListAsync();
        }

        // TODO make it async
        public async Task<IndexViewModel> FilterAsync(int countryId, int isDeleted, int? take = null, int skip = 0)
        {
            var query = this.GetAllInCountryAsIQueryable(countryId);

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

            var resultsCount = query.Count();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && query.Count() > take)
            {
                query = query.Take(take.Value);
            }

            var viewModel = new IndexViewModel
            {
                ResultsCount = resultsCount,
                CountryId = countryId,
                Cities = await query.OrderBy(c => c.Name).To<InfoViewModel>().ToListAsync(),
                Location = this.countriesService.GetNameById(countryId),
                Filter = new FilterBarViewModel
                {
                    IsDeleted = isDeleted,
                },
            };

            return viewModel;
        }

        public T GetById<T>(int cityId)
        {
            return this.GetAsIQueriable(cityId)
                .To<T>()
                .FirstOrDefault();
        }

        public async Task CreateAsync(string cityName, int countryId)
        {
            var city = new City
            {
                Name = cityName,
                CountryId = countryId,
            };

            await this.citiesRepository.AddAsync(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, string name, int countryId, bool isDeleted)
        {
            var city = this.GetAsIQueriable(id).FirstOrDefault();
            city.Name = name;
            city.CountryId = countryId;
            city.IsDeleted = isDeleted;

            this.citiesRepository.Update(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var city = this.GetAsIQueriable(id).FirstOrDefault();
            this.citiesRepository.Delete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public int GetCountInCountry(int countryId)
        {
            return this.GetAllInCountryAsIQueryable(countryId).Count();
        }

        // Helpers
        private IQueryable<City> GetAsIQueriable(int cityId)
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

        private IQueryable<City> GetAllInCountryAsIQueryable(int countryId)
        {
            return this.citiesRepository
                .All()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Name);
        }
    }
}
