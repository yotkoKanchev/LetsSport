namespace LetsSport.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Messages;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class EventsController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly IEventsService eventsService;
        private readonly IMessagesService messagesService;
        private readonly ISportsService sportsService;
        private readonly UserManager<ApplicationUser> userManager;

        public EventsController(
            IArenasService arenasService,
            IEventsService eventsService,
            IMessagesService messagesService,
            ISportsService sportsService,
            ILocationLocator locationLocator,
            UserManager<ApplicationUser> userManager)
            : base(locationLocator)
        {
            this.arenasService = arenasService;
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
            var administratingEvents = await this.eventsService.GetAllAdministratingEventsByUserId<_EventCardPartialViewModel>(userId, country);
            var participatingEvents = await this.eventsService.GetUpcomingEvents<_EventCardPartialViewModel>(userId, country, 8);
            var canceledEvents = await this.eventsService.GetCanceledEvents<_EventCardPartialViewModel>(userId, country, 4);

            var viewModel = new EventsIndexMyEventsViewModel
            {
                ParticipatingEvents = participatingEvents,
                AdministratingEvents = administratingEvents,
                CanceledEvents = canceledEvents,
            };

            return this.View(viewModel);
        }

        public IActionResult Create()
        {
            // TODO pass sportType to GetArenas to filter them by SportType
            var location = this.GetLocation();

            var viewModel = new EventCreateInputModel
            {
                Arenas = this.arenasService.GetAllArenas(location),
                Sports = this.sportsService.GetAllSportsInCountry(location.Country),
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

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> AddUser(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.AddUserAsync(id, user.Id, user.Email, user.UserName);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> RemoveUser(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.RemoveUserAsync(id, user);
            return this.Redirect($"/");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user.AdministratingEvents.Any(e => e.Id == id))
            {
                return this.Unauthorized();
            }

            await this.eventsService.CancelEvent(id, user.Email, user.UserName);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
