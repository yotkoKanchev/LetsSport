namespace LetsSport.Web.Controllers
{
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class AboutController : BaseController
    {
        public AboutController(ILocationLocator locationLocator)
            : base(locationLocator)
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
