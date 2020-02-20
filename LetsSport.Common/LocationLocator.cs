namespace LetsSport.Common
{
    using System.Globalization;
    using System.Net;

    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class LocationLocator : ILocationLocator
    {
        private IHttpContextAccessor accessor;

        public LocationLocator(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public (string Country, string City) GetLocationInfo()
        {
            //var ipAddress = this.accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            string info = new WebClient().DownloadString("http://ipinfo.io/" /*+ ipAddress*/);
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
