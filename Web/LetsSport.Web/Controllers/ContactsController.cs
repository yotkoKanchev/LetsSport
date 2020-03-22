namespace LetsSport.Web.Controllers
{
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Mvc;

    public class ContactsController : BaseController
    {
        public ContactsController(ILocationLocator locationLocator)
            : base(locationLocator)
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
