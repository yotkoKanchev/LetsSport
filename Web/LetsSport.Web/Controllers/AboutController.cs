namespace LetsSport.Web.Controllers
{
    using LetsSport.Web.Filters;
    using Microsoft.AspNetCore.Mvc;

    [ServiceFilter(typeof(SetLocationResourceFilter))]
    public class AboutController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
