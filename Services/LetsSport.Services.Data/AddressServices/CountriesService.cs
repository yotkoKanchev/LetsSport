namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
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
    }
}
