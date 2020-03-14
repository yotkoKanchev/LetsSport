namespace LetsSport.Web.Controllers
{
    using LetsSport.Services.Data.Common;
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
            string currentCity;
            string currentCountry;

            if (this.HttpContext.Session.GetString("city") != null)
            {
                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
                currentCity = this.HttpContext.Session.GetString("city");
                currentCountry = this.HttpContext.Session.GetString("country");
            }
            else
            {
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                var currentLocation = this.locator.GetLocationInfo(ip);
                currentCity = currentLocation.City;
                currentCountry = currentLocation.Country;
                var location = currentLocation.City + ", " + currentLocation.Country;

                this.HttpContext.Session.SetString("city", currentCity);
                this.HttpContext.Session.SetString("country", currentCountry);
                this.HttpContext.Session.SetString("location", location);

                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
            }
        }
    }
}
