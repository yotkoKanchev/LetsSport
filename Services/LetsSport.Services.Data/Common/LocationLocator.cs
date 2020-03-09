namespace LetsSport.Services.Data.Common
{
    using System.Globalization;
    using System.Net;

    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class LocationLocator : ILocationLocator
    {
        private readonly IConfiguration configuration;

        public LocationLocator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public (string Country, string City) GetLocationInfo(string ip)
        {
            // if in Development remove localhost ip
            if (ip == "[::1]")
            {
                ip += '/';
            }
            else
            {
                ip = string.Empty;
            }

            var key = this.configuration["IpInfo:ApiKey"];
            var path = "http://ipinfo.io/" + ip + $"?token={key}";
            string info = new WebClient().DownloadString(path);
            var ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo countryInfo = new RegionInfo(ipInfo.Country);
            var countryName = countryInfo.EnglishName;
            var cityName = ipInfo.City;
            return (countryName, cityName);
        }

        private class IpInfo
        {
            [JsonProperty("city")]
            public string City { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("postal")]
            public string Postal { get; set; }
        }
    }
}
