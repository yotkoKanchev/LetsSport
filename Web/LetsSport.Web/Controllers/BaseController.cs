namespace LetsSport.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        protected (string City, string Country) GetLocation()
        {
            var city = this.HttpContext.Session.GetString("city");
            var country = this.HttpContext.Session.GetString("country");
            return (city, country);
        }
    }
}
