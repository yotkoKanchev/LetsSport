namespace LetsSport.Web.Controllers
{
    using LetsSport.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected (string City, string Country) GetLocation()
        {
            var city = this.HttpContext.Session.GetString(GlobalConstants.City);
            var country = this.HttpContext.Session.GetString(GlobalConstants.Country);

            return (city, country);
        }

        // another way to obtain ip
        private string GetUserIP()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
        }
    }
}
