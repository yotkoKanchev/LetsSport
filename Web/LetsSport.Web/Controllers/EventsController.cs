namespace LetsSport.Web.Controllers
{
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        public IActionResult Create()
        {
            // TODO pass filtered by sport Arenas with AJAX;
            //var arenas = this.arenasService.GetAll();
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(EventCreateInputModel inputModel)
        {
            var model = inputModel;
            // TODO pass filtered by sport Arenas with AJAX;
            return this.View();
        }
    }
}
