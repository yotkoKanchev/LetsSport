namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
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
            var country = this.countriesRepository
                .AllAsNoTracking()
                .Where(c => c.Name == countryName)
                .FirstOrDefault();

            return country.Id;
        }
    }
}
