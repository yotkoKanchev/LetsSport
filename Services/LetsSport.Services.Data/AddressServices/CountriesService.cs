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
            var countries = this.countriesRepository.All();
            var resultList = new List<SelectListItem>();

            foreach (var country in countries)
            {
                resultList.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
            }

            return resultList;
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
    }
}
