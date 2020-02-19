namespace LetsSport.Web.Controllers
{

    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        public IActionResult Create()
        {
            return this.View();
        }
    }
}
