namespace LetsSport.Common
{
    using System.Globalization;
    using System.Net;

    using Newtonsoft.Json;

    public static class CurrentLocation
    {
        public static (string Country, string City) GetLocationInfo()
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/");
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
