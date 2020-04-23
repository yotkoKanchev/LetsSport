namespace LetsSport.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        private readonly string city = "city";
        private readonly string country = "country";

        protected (string City, string Country) GetLocation()
        {
            var city = this.HttpContext.Session.GetString(this.city);
            var country = this.HttpContext.Session.GetString(this.country);

            return (city, country);
        }

        // another way to obtain ip
        private string GetUserIP()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
        }
    }
}
