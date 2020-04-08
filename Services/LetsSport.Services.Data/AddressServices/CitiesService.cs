namespace LetsSport.Services.Data.AddressServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class CitiesService : ICitiesService
    {
        private readonly IDeletableEntityRepository<City> citiesRepository;
        private readonly ICountriesService countriesService;

        public CitiesService(IDeletableEntityRepository<City> citiesRepository, ICountriesService countriesService)
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

        public async Task<bool> IsExistsAsync(string cityName, int countryId)
        {
            return await this.citiesRepository.All()
                .AnyAsync(c => c.Name == cityName && c.Country.Id == countryId);
        }

        // TODO remove this dummy method
        public async Task<string> GetLocationByCityIdAsync(int cityId)
        {
            return await this.GetAsIQueriable(cityId)
                .Select(c => c.Name + ", " + c.Country.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetNameByIdAsync(int cityId)
        {
            return await this.GetAsIQueriable(cityId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        // Admin
        public async Task<IEnumerable<T>> GetAllByCountryIdAsync<T>(int countryId, int? take = null, int skip = 0)
        {
            var query = this.
                citiesRepository
                .AllWithDeleted()
                .OrderBy(c => c.DeletedOn)
                .ThenBy(c => c.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query
              .To<T>()
              .ToListAsync();
        }

        public async Task<IndexViewModel> FilterAsync(int countryId, int isDeleted, int? take = null, int skip = 0)
        {
            var query = this.citiesRepository
                .AllWithDeleted()
                .OrderBy(c => c.DeletedOn)
                .ThenBy(c => c.Name)
                .Where(c => c.CountryId == countryId);

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

            var resultCount = await query.CountAsync();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var viewModel = new IndexViewModel
            {
                ResultCount = resultCount,
                CountryId = countryId,
                Cities = await query.OrderBy(c => c.Name).To<InfoViewModel>().ToListAsync(),
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Filter = new SimpleModelsFilterBarViewModel
                {
                    IsDeleted = isDeleted,
                },
            };

            return viewModel;
        }

        public async Task<T> GetByIdAsync<T>(int cityId)
        {
            return await this.GetAsIQueriableInclDeleted(cityId)
                .To<T>()
                .FirstOrDefaultAsync();
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
            var city = await this.GetAsIQueriableInclDeleted(id).FirstAsync();

            city.Name = name;
            city.CountryId = countryId;
            city.IsDeleted = isDeleted;

            this.citiesRepository.Update(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task ArchiveByIdAsync(int id)
        {
            var city = await this.GetAsIQueriableInclDeleted(id).FirstAsync();
            this.citiesRepository.Delete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var city = await this.GetAsIQueriableInclDeleted(id).FirstAsync();
            this.citiesRepository.HardDelete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task<int> GetCountInCountryAsync(int countryId)
        {
            return await this.GetAllInCountryAsIQueryable(countryId).CountAsync();
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

        private IQueryable<City> GetAsIQueriableInclDeleted(int cityId)
        {
            var city = this.citiesRepository
                .AllWithDeleted()
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
