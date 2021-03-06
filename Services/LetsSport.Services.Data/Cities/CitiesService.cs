﻿namespace LetsSport.Services.Data.Cities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Countries;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;

    public class CitiesService : ICitiesService
    {
        private readonly ICountriesService countriesService;
        private readonly IDeletableEntityRepository<City> citiesRepository;

        public CitiesService(ICountriesService countriesService, IDeletableEntityRepository<City> citiesRepository)
        {
            this.countriesService = countriesService;
            this.citiesRepository = citiesRepository;
        }

        public async Task<int> GetIdAsync(string cityName, int countryId)
        {
            var query = this.citiesRepository
                .All()
                .OrderByDescending(c => c.ModifiedOn);

            var id = await this.GetAllInCountryAsIQueryable(countryId)
                .Where(c => c.Name == cityName)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (id == 0)
            {
                throw new ArgumentException(string.Format(CityInvalidNameErrorMesage, cityName, countryId));
            }

            return id;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId)
            => await this.GetAllInCountryAsIQueryable(countryId)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
                .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetAllWithEventsInCountryAsync(int countryId)
            => await this.GetAllInCountryAsIQueryable(countryId)
                 .Where(c => c.Events.Any())
                 .OrderBy(c => c)
                 .Select(c => new SelectListItem
                 {
                     Text = c.Name,
                     Value = c.Id.ToString(),
                 })
                 .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetAllWithArenasInCountryAsync(int countryId)
            => await this.GetAllInCountryAsIQueryable(countryId)
                .Where(c => c.Arenas.Any())
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                })
                .ToListAsync();

        public async Task<bool> IsExistsAsync((string CityName, string CountryName) location)
        {
            var countryId = await this.countriesService.GetIdAsync(location.CountryName);
            return await this.citiesRepository
                .All()
                .AnyAsync(c => c.Name == location.CityName &&
                               c.Country.Id == countryId);
        }

        public async Task<string> GetNameByIdAsync(int cityId)
            => await this.GetAsIQueriable(cityId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

        // Admin
        public async Task<IEnumerable<T>> GetAllByCountryIdAsync<T>(int countryId, int? take = null, int skip = 0)
        {
            var query = this.citiesRepository
                .AllWithDeleted()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.IsDeleted)
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

        public async Task<IndexViewModel> FilterAsync(int countryId, int deletionStatus, int? take = null, int skip = 0)
        {
            IQueryable<City> query = this.citiesRepository
                .AllWithDeleted()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.DeletedOn)
                .ThenBy(c => c.Name);

            if (deletionStatus != 0)
            {
                if (deletionStatus == 1)
                {
                    query = query
                        .Where(c => c.IsDeleted == false);
                }
                else if (deletionStatus == 2)
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
                Cities = await query.OrderBy(c => c.Name).To<CityInfoViewModel>().ToListAsync(),
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Filter = new SimpleModelsFilterBarViewModel
                {
                    DeletionStatus = deletionStatus,
                },
            };

            return viewModel;
        }

        public async Task<T> GetByIdAsync<T>(int cityId)
        {
            var query = this.GetAsIQueriableInclDeleted(cityId);

            return await query
                    .To<T>()
                    .FirstOrDefaultAsync();
        }

        public async Task CreateAsync((string CityName, int CountryId) location)
        {
            var countryId = location.CountryId;

            if (this.citiesRepository
                .AllWithDeleted()
                .Any(c => c.CountryId == countryId && c.Name == location.CityName)
                    || await this.countriesService.IsValidId(countryId) == false)
            {
                throw new ArgumentException(string.Format(CityExistsMessage, location.CityName, countryId));
            }

            var city = new City
            {
                Name = location.CityName,
                CountryId = countryId,
            };

            await this.citiesRepository.AddAsync(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, string name, int countryId, bool isDeleted)
        {
            if (this.citiesRepository
                .AllWithDeleted()
                .Any(c => c.CountryId == countryId && c.Name == name && c.Id != id))
            {
                throw new ArgumentException(string.Format(CityExistsMessage, name, countryId));
            }

            var city = await this.GetAsIQueriableInclDeleted(id).FirstOrDefaultAsync();

            city.Name = name;
            city.CountryId = countryId;
            city.IsDeleted = isDeleted;

            if (isDeleted == false)
            {
                city.DeletedOn = null;
            }

            this.citiesRepository.Update(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task ArchiveByIdAsync(int id)
        {
            var city = await this.GetAsIQueriableInclDeleted(id)
                .FirstOrDefaultAsync();
            this.citiesRepository.Delete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var city = await this.GetAsIQueriableInclDeleted(id)
                .FirstOrDefaultAsync();
            this.citiesRepository.HardDelete(city);
            await this.citiesRepository.SaveChangesAsync();
        }

        public async Task<int> GetCountInCountryAsync(int countryId)
            => await this.GetAllInCountryAsIQueryable(countryId)
                .CountAsync();

        // Helpers
        private IQueryable<City> GetAsIQueriable(int cityId)
            => this.citiesRepository
                .All()
                .Where(c => c.Id == cityId);

        private IQueryable<City> GetAsIQueriableInclDeleted(int cityId)
            => this.citiesRepository
                .AllWithDeleted()
                .Where(c => c.Id == cityId);

        private IQueryable<City> GetAllInCountryAsIQueryable(int countryId)
            => this.citiesRepository
                .All()
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Name);
    }
}
