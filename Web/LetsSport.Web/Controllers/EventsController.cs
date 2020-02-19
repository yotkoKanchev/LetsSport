namespace LetsSport.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        public IActionResult Create()
        {
            // TODO pass filtered by sport Arenas with AJAX;
            return this.View();
        }
    }
}
