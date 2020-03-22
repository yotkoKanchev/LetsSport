namespace LetsSport.Web.Infrastructure
{
    using System.Globalization;
    using System.Net;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.Extensions.Configuration;

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
            var ipInfo = JsonSerializer.Deserialize<IpInfo>(info);
            RegionInfo countryInfo = new RegionInfo(ipInfo.Country);
            var countryName = countryInfo.EnglishName;
            var cityName = ipInfo.City;
            return (countryName, cityName);
        }

        private class IpInfo
        {
            [JsonPropertyName("city")]
            public string City { get; set; }

            [JsonPropertyName("region")]
            public string Region { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }

            [JsonPropertyName("postal")]
            public string Postal { get; set; }
        }
    }
}
