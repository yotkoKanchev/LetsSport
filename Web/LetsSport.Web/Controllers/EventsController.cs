namespace LetsSport.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Messages;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class EventsController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly IArenasService arenasService;
        private readonly IEventsService eventsService;
        private readonly IMessagesService messagesService;
        private readonly ISportsService sportsService;
        private readonly UserManager<ApplicationUser> userManager;

        public EventsController(
            ICitiesService citiesService,
            ICountriesService countriesService,
            IArenasService arenasService,
            IUsersService usersService,
            IEventsService eventsService,
            IMessagesService messagesService,
            ISportsService sportsService,
            ILocationLocator locationLocator,
            UserManager<ApplicationUser> userManager)
            : base(locationLocator)
        {
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.arenasService = arenasService;
            this.usersService = usersService;
            this.eventsService = eventsService;
            this.messagesService = messagesService;
            this.sportsService = sportsService;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);
            var country = this.GetLocation().Country;
            var administratingEvents = await this.eventsService.GetAllAdministratingEventsByUserId<EventCardPartialViewModel>(userId, country);
            var participatingEvents = await this.eventsService.GetUpcomingEvents<EventCardPartialViewModel>(userId, country, 8);
            var canceledEvents = await this.eventsService.GetCanceledEvents<EventCardPartialViewModel>(userId, country, 4);

            var viewModel = new EventsIndexMyEventsViewModel
            {
                ParticipatingEvents = participatingEvents,
                AdministratingEvents = administratingEvents,
                CanceledEvents = canceledEvents,
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            // TODO find a way to let user choose in wich city to create event
            var location = this.GetLocation();
            var cityId = await this.citiesService.GetCityIdAsync(location);
            var countryId = this.countriesService.GetCountryId(location.Country);
            var viewModel = new EventCreateInputModel
            {
                Arenas = this.arenasService.GetAllArenasInCitySelectList(cityId),
                Sports = this.sportsService.GetAllSportsInCity(cityId),
                Date = DateTime.UtcNow,
                CityId = cityId,
                CountryId = countryId,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                inputModel.Arenas = this.arenasService.GetAllArenas(location);
                inputModel.Sports = this.sportsService.GetAllSportsInCountry(location.Country);
                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var id = await this.eventsService.CreateAsync(inputModel, user.Id, user.Email, user.UserName);
            this.TempData["message"] = $"Your event has been created successfully!";
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var viewModel = this.eventsService.GetDetailsWithChatRoom(id, user?.Id, user?.UserName);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Details(MessageCreateInputModel inputModel, int id)
        {
            if (this.ModelState.IsValid)
            {
                var userId = this.userManager.GetUserId(this.User);
                await this.messagesService.CreateMessageAsync(inputModel.MessageContent, userId, id);
            }

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public IActionResult Edit(int id)
        {
            var location = this.GetLocation();
            var inputModel = this.eventsService.GetDetailsForEdit(id, location);

            var userId = this.userManager.GetUserId(this.User);

            if (userId != inputModel.AdminId)
            {
                return new ForbidResult();
            }

            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel viewModel)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (userId != viewModel.AdminId)
            {
                return new ForbidResult();
            }

            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                var inputModel = this.eventsService.GetDetailsForEdit(viewModel.Id, location);

                return this.View(inputModel);
            }

            await this.eventsService.UpdateEvent(viewModel);
            var id = viewModel.Id;

            this.TempData["message"] = $"Your event has been updated successfully!";
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> AddUser(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.AddUserAsync(id, user.Id, user.Email, user.UserName);

            this.TempData["message"] = $"You joined the event successfully!";
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> RemoveUser(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.RemoveUserAsync(id, user);
            this.TempData["message"] = $"You left the event successfully!";

            return this.RedirectToAction(nameof(this.Details), new { id });

            // return this.Redirect($"/");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user.AdministratingEvents.Any(e => e.Id == id))
            {
                return this.Unauthorized();
            }

            await this.eventsService.CancelEvent(id, user.Email, user.UserName);
            this.TempData["message"] = $"You cancel the event successfully!";

            return this.RedirectToAction(nameof(this.Details), new { id });

            // return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Invite(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var invitedUsersCount = await this.eventsService.InviteUsersToEvent(id, user.Email, user.UserName);

            return this.View(invitedUsersCount);
        }
    }
}
