namespace LetsSport.Web.Controllers
{
    using System.Linq;

    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        private readonly ILocationLocator locator;

        public BaseController(ILocationLocator locator)
        {
            this.locator = locator;
        }

        protected (string City, string Country) GetLocation()
        {
            var city = this.HttpContext.Session.GetString("city");
            var country = this.HttpContext.Session.GetString("country");
            return (city, country);
        }

        protected void SetLocation()
        {
            if (!this.HttpContext.Session.Keys.Contains("country")
                || !this.HttpContext.Session.Keys.Contains("city")
                || !this.HttpContext.Session.Keys.Contains("location"))
            {
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                var currentLocation = this.locator.GetLocationInfo(ip);
                var currentCity = currentLocation.City;
                var currentCountry = currentLocation.Country;
                var location = currentLocation.City + ", " + currentLocation.Country;

                this.HttpContext.Session.SetString("city", currentCity);
                this.HttpContext.Session.SetString("country", currentCountry);
                this.HttpContext.Session.SetString("location", location);
            }
        }
    }
}
