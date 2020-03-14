namespace LetsSport.Web.Controllers
{
    using LetsSport.Services.Data.Common;
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
