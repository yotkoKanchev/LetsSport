namespace LetsSport.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
