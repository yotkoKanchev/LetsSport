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
            if (!this.HttpContext.Session.Keys.Contains("country"))
            {
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                var (country, city) = this.locator.GetLocationInfo(ip);

                this.HttpContext.Session.SetString("city", city);
                this.HttpContext.Session.SetString("country", country);
            }
        }
    }
}
