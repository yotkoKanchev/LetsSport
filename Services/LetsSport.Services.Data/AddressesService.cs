namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.AddressModels;
    using Newtonsoft.Json;

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
            var currentCity = this.GetCurrentLocation().City;

            if (!this.db.Cities.Any(c => c.Name == currentCity))
            {
                var city = new City
                {
                    Name = currentCity,
                    CountryId = this.db.Countries.Where(c => c.Name == this.GetCurrentLocation().Country).Select(c => c.Id).First(),
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

        private (string Country, string City) GetCurrentLocation()
        {
            IpInfo ipInfo = new IpInfo();
            string info = new WebClient().DownloadString("http://ipinfo.io/");
            ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo countryInfo = new RegionInfo(ipInfo.Country);
            var country = countryInfo.EnglishName;
            var city = ipInfo.City;

            return (country, city);
        }

        private class IpInfo
        {
            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("hostname")]
            public string Hostname { get; set; }

            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("loc")]
            public string Loc { get; set; }

            [JsonProperty("org")]
            public string Org { get; set; }

            [JsonProperty("postal")]
            public string Postal { get; set; }
        }
    }
}
