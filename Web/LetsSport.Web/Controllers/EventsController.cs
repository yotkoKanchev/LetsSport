﻿namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;

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
            // TODO pass sportType to GetArenas to filter them by SportType
            var arenas = this.arenasService.GetArenas();
            this.ViewData["arenas"] = arenas;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Error");
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.eventsService.CreateAsync(inputModel, userId);
            return this.Redirect("/");
        }

        public IActionResult Details(int id)
        {
            var inputModel = this.eventsService.GetEvent(id);
            return this.View(inputModel);
        }

        public IActionResult Edit(int id)
        {
            var inputModel = this.eventsService.GetDetailsForEdit(id);
            return this.View(inputModel);
        }

        [HttpPost]
        public IActionResult Edit(EventEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Error");
            }

            this.eventsService.UpdateEvent(viewModel);

            var eventId = viewModel.Id;
            return this.Redirect($"/Events/Details/{eventId}");
        }

        public async Task<IActionResult> AddUser(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.eventsService.AddUserAsync(id, userId);

            return this.Redirect($"/events/details/{id}");
        }

        public async Task<IActionResult> RemoveUser(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.eventsService.RemoveUserAsync(id, userId);

            return this.Redirect($"/events/details/{id}");
        }
    }
}
