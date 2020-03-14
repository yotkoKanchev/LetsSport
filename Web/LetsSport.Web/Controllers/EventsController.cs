namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;


    [Authorize]
    public class EventsController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly IEventsService eventsService;
        private readonly IMessagesService messagesService;
        private readonly ISportsService sportsService;

        public EventsController(
            IArenasService arenasService,
            IEventsService eventsService,
            IMessagesService messagesService,
            ISportsService sportsService,
            ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.arenasService = arenasService;
            this.eventsService = eventsService;
            this.messagesService = messagesService;
            this.sportsService = sportsService;
        }

        public IActionResult Create()
        {
            // TODO pass sportType to GetArenas to filter them by SportType
            this.SetLocation();
            var location = this.GetLocation();

            var viewModel = new EventCreateInputModel
            {
                Arenas = this.arenasService.GetArenas(location),
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
                inputModel.Arenas = this.arenasService.GetArenas(location);
                inputModel.Sports = this.sportsService.GetAllSportsInCountry(location.Country);
                return this.View(inputModel);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventId = await this.eventsService.CreateAsync(inputModel, userId/*, currentCity, currentCountry*/);
            return this.Redirect($"Details/{eventId}");
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var viewModel = this.eventsService.GetDetailsWithChatRoom(id);
            this.TempData["chatRoomId"] = viewModel.Id;

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Details(MessageCreateInputModel inputModel, int id)
        {
            var chatRoomId = this.TempData["chatRoomId"].ToString();

            if (this.ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await this.messagesService.CreateMessageAsync(inputModel.MessageContent, userId, id);
            }

            return this.Redirect($"/events/details/{id}");
        }

        public IActionResult Edit(int id)
        {
            this.SetLocation();
            var location = this.GetLocation();
            var inputModel = this.eventsService.GetDetailsForEdit(id, location);
            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                var inputModel = this.eventsService.GetDetailsForEdit(viewModel.Id, location);

                return this.View(inputModel);
            }

            await this.eventsService.UpdateEvent(viewModel);

            var eventId = viewModel.Id;
            return this.Redirect($"/events/details/{eventId}");
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
