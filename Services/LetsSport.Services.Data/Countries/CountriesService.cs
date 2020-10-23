namespace LetsSport.Services.Data.Countries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
            => await this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
                .ToListAsync();

        public async Task<int> GetIdAsync(string countryName)
        {
            var countryId = await this.countriesRepository
                .All()
                .Where(c => c.Name == countryName)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (countryId == 0)
            {
                throw new ArgumentException(string.Format(CountryInvalidNameErrorMessage, countryName));
            }

            return countryId;
        }

        public async Task<string> GetNameByIdAsync(int countryId)
            => await this.GetCountryAsIQueryable(countryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

        public async Task<T> GetByIdAsync<T>(int id)
        {
            var country = this.GetCountryAsIQueryable(id);

            return await country.To<T>().FirstOrDefaultAsync();
        }

        // Admin
        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.countriesRepository
                .All()
                .OrderBy(s => s.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<int> GetCountAsync()
            => await this.countriesRepository
                .All()
                .CountAsync();

        public async Task<int> CreateAsync(string name)
        {
            if (this.countriesRepository.All().Any(c => c.Name == name))
            {
                throw new ArgumentException(string.Format(CountryExistsMessage, name));
            }

            var country = new Country
            {
                Name = name,
            };

            await this.countriesRepository.AddAsync(country);
            await this.countriesRepository.SaveChangesAsync();

            return country.Id;
        }

        public async Task UpdateAsync(int id, string name)
        {
            if (await this.countriesRepository.All().AnyAsync(c => c.Name == name && c.Id == id)
                || name == null)
            {
                throw new ArgumentException(string.Format(CountryExistsMessage, name));
            }

            var country = await this.GetCountryAsIQueryable(id).FirstOrDefaultAsync();

            country.Name = name;

            this.countriesRepository.Update(country);
            await this.countriesRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var country = await this.GetCountryAsIQueryable(id).FirstOrDefaultAsync();
            this.countriesRepository.Delete(country);
            await this.countriesRepository.SaveChangesAsync();
        }

        public async Task<bool> IsValidId(int countryId)
            => await this.countriesRepository
                .All()
                .AnyAsync(c => c.Id == countryId);

        private IQueryable<Country> GetCountryAsIQueryable(int id)
            => this.countriesRepository
                .All()
                .Where(s => s.Id == id);
    }
}
