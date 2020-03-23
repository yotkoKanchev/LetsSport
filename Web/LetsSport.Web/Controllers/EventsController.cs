namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Messages;
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
            var location = this.GetLocation();
            var administratingEvents = await this.eventsService.GetAllAdministratingEventsByUserId<HomeEventInfoViewModel>(userId, location.Country);
            var participatingEvents = await this.eventsService.GetParticipatingEvents<HomeEventInfoViewModel>(userId, location.Country, 8);

            var viewModel = new EventsIndexMyEventsViewModel
            {
                ParticipatingEvents = participatingEvents,
                AdministratingEvents = administratingEvents,
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
        public IActionResult Details(int id)
        {
            var viewModel = this.eventsService.GetDetailsWithChatRoom(id);

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
            await this.eventsService.RemoveUserAsync(id, user.Id, user.Email, user.UserName);
            return this.Redirect($"/");
        }
    }
}
