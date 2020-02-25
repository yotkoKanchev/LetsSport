namespace LetsSport.Services.Data.AddressServices
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public async Task<int> GetCountryIdAsync(string countryName)
        {
            var country = this.countriesRepository
                .AllAsNoTracking()
                .Where(c => c.Name == countryName)
                .FirstOrDefault();

            if (country == null)
            {
                country = new Country
                {
                    Name = countryName,
                };

                await this.countriesRepository.AddAsync(country);
                await this.countriesRepository.SaveChangesAsync();
            }

            return country.Id;
        }
    }
}
