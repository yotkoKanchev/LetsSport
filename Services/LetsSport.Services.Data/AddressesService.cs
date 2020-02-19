namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data;
    using LetsSport.Data.Models.AddressModels;

    public class AddressesService : IAddressesService
    {
        private readonly ApplicationDbContext db;

        public AddressesService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Create(string country, string city, string addressFromInput)
        {
            var countryId = this.GetCountryId(country);
            var cityId = await this.GetCityId(city, countryId);

            var address = new Address
            {
                CityId = cityId,
                StreetAddress = addressFromInput,
                CreatedOn = DateTime.UtcNow,
            };

            await this.db.Addresses.AddAsync(address);
            await this.db.SaveChangesAsync();

            return address.Id;
        }

        public IEnumerable<string> GetCities()
        {
            var currentCity = CurrentLocation.GetCurrentCity();

            if (!this.db.Cities.Any(c => c.Name == currentCity))
            {
                var city = new City
                {
                    Name = currentCity,
                    CountryId = this.db.Countries.Where(c => c.Name == CurrentLocation.GetCountry()).Select(c => c.Id).First(),
                    CreatedOn = DateTime.UtcNow,
                };

                this.db.Cities.AddAsync(city);
                this.db.SaveChangesAsync();
            }

            var cities = this.db.Cities.Select(c => c.Name).ToList();

            return cities;
        }

        private async Task<int> GetCityId(string cityName, int countryId)
        {
            var city = this.db.Cities
                .Where(c => c.Name == cityName && c.CountryId == countryId)
                .FirstOrDefault();

            if (city == null)
            {
                city = new City
                {
                    CountryId = countryId,
                    Name = cityName,
                    CreatedOn = DateTime.UtcNow,
                };

                await this.db.Cities.AddAsync(city);
                await this.db.SaveChangesAsync();
            }

            return city.Id;
        }

        private int GetCountryId(string country)
        {
            var countryId = this.db.Countries
                .Where(c => c.Name == country)
                .Select(c => c.Id)
                .FirstOrDefault();

            return countryId;
        }
    }
}
