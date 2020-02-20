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
        private readonly ILocationLocator locator;

        public AddressesService(ApplicationDbContext db, ILocationLocator locator)
        {
            this.db = db;
            this.locator = locator;
        }

        public async Task<int> Create(string country, string city, string addressFromInput)
        {
            var countryId = await this.GetCountryId(country);
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
            var currentLocation = this.locator.GetLocationInfo();
            var cityName = currentLocation.City;
            var country = currentLocation.Country;
            if (!this.db.Cities.Any(c => c.Name == cityName && c.Country.Name == country))
            {
                var countryId = this.db.Countries.Where(c => c.Name == country).Select(c => c.Id).FirstOrDefault();

                var city = new City
                {
                    Name = cityName,
                    CountryId = countryId,
                    CreatedOn = DateTime.UtcNow,
                };

                this.db.Cities.AddAsync(city);
                this.db.SaveChangesAsync();
            }

            var cities = this.db.Cities
                .Where(c => c.Country.Name == country)
                .Select(c => c.Name)
                .ToList();

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

        private async Task<int> GetCountryId(string countryName)
        {
            var country = this.db.Countries
                .Where(c => c.Name == countryName)
                .FirstOrDefault();

            if (country == null)
            {
                country = new Country
                {
                    Name = countryName,
                    CreatedOn = DateTime.UtcNow,
                };

                await this.db.Countries.AddAsync(country);
                await this.db.SaveChangesAsync();
            }

            return country.Id;
        }
    }
}
