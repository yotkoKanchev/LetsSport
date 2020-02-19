namespace LetsSport.Common
{
    using System.Globalization;
    using System.Net;

    using Newtonsoft.Json;

    public static class CurrentLocation
    {
        public static string GetCurrentCity()
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/");
            var ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            var city = ipInfo.City;

            return city;
        }

        public static string GetCountry()
        {
            string info = new WebClient().DownloadString("http://ipinfo.io/");
            var ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
            RegionInfo countryInfo = new RegionInfo(ipInfo.Country);
            var country = countryInfo.EnglishName;

            return country;
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
