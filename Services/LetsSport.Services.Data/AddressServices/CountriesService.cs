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
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;
        private readonly IRepository<Arena> arenasRepository;

        public CountriesService(IRepository<Country> countriesRepository, IRepository<Arena> arenasRepository)
        {
            this.countriesRepository = countriesRepository;
            this.arenasRepository = arenasRepository;
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var countries = this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });

            return countries;
        }

        public int GetCountryId(string countryName)
        {
            var countryId = this.countriesRepository
                .All()
                .Where(c => c.Name == countryName)
                .Select(c => c.Id)
                .FirstOrDefault();

            return countryId;
        }

        public int GetCountryIdByArenaId(int arenaId)
        {
            return this.arenasRepository
                 .All()
                 .Where(a => a.Id == arenaId)
                 .Select(a => a.CountryId)
                 .FirstOrDefault();
        }

        public string GetCountryNameById(int countryId)
        {
            return this.countriesRepository
                .All()
                .Where(c => c.Id == countryId)
                .Select(c => c.Name)
                .FirstOrDefault();
        }

        // Admin
        public IEnumerable<T> GetAllAsIQueryable<T>()
        {
            return this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .To<T>()
                .ToList();
        }

        public async Task<int> CreateCountry(string name)
        {
            var country = new Country
            {
                Name = name,
            };

            await this.countriesRepository.AddAsync(country);
            await this.countriesRepository.SaveChangesAsync();

            return country.Id;
        }

        public T GetCountryById<T>(int id)
        {
            var sport = this.GetCountryAsIQueryable(id).FirstOrDefault();

            return sport.To<T>();
        }

        public async Task UpdateCountry(int id, string name)
        {
            var country = this.GetCountryAsIQueryable(id).FirstOrDefault();

            country.Name = name;

            this.countriesRepository.Update(country);
            await this.countriesRepository.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
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
