namespace LetsSport.Services.Data.AddressServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public IEnumerable<SelectListItem> GetAllAsSelectList()
        {
            var countries = this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
                .ToList();

            return countries;
        }

        public int GetId(string countryName)
        {
            var countryId = this.countriesRepository
                .All()
                .Where(c => c.Name == countryName)
                .Select(c => c.Id)
                .FirstOrDefault();

            if (countryId == 0)
            {
                throw new ArgumentException($"Country with name: {countryName} does not exists!");
            }

            return countryId;
        }

        public string GetNameById(int countryId)
        {
            return this.GetCountryAsIQueryable(countryId)
                .Select(c => c.Name)
                .FirstOrDefault();
        }

        public T GetById<T>(int id)
        {
            var sport = this.GetCountryAsIQueryable(id).FirstOrDefault();

            return sport.To<T>();
        }

        // Admin
        public IEnumerable<T> GetAll<T>(int? take = null, int skip = 0)
        {
            var query = this.countriesRepository
                .All()
                .OrderBy(s => s.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>()
                .ToList();
        }

        public int GetCount()
        {
            return this.countriesRepository.All().Count();
        }

        public async Task<int> CreateAsync(string name)
        {
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
            var country = this.GetCountryAsIQueryable(id).FirstOrDefault();

            country.Name = name;

            this.countriesRepository.Update(country);
            await this.countriesRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var country = this.GetCountryAsIQueryable(id).FirstOrDefault();
            this.countriesRepository.Delete(country);
            await this.countriesRepository.SaveChangesAsync();
        }

        private IQueryable<Country> GetCountryAsIQueryable(int id)
        {
            var country = this.countriesRepository
                .All()
                .Where(s => s.Id == id);

            if (country == null)
            {
                throw new ArgumentException($"Country with ID: {id} does not exists!");
            }

            return country;
        }
    }
}
