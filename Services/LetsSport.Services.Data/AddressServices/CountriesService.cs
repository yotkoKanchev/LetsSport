namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public IEnumerable<string> GetAll()
        {
            var countryNames = this.countriesRepository
                .All()
                .Select(c => c.Name)
                .ToList();

            return countryNames;
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
