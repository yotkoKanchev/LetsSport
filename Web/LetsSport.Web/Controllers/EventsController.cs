namespace LetsSport.Web.Controllers
{
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class EventsController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly IEventsService eventsService;

        public EventsController(IArenasService arenasService, IEventsService eventsService)
        {
            this.arenasService = arenasService;
            this.eventsService = eventsService;
        }

        public IActionResult Create()
        {
            var arenas = this.arenasService.GetArenas();
            this.ViewData["arenas"] = arenas;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateInputModel inputModel)
        {
            var model = inputModel;

            if (!this.ModelState.IsValid)
            {
                return this.View("Error");

            }

            await this.eventsService.CreateAsync(inputModel);
            return this.Redirect("/");

        }
    }
}
