namespace LetsSport.Web.Controllers
{
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;

    public class EventsController : BaseController
    {
        private readonly IArenasService arenasService;

        public EventsController(IArenasService arenasService)
        {
            this.arenasService = arenasService;
        }

        public IActionResult Create()
        {
            var arenas = this.arenasService.GetArenas();
            this.ViewData["arenas"] = arenas;
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
