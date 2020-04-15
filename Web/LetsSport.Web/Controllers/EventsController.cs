namespace LetsSport.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.GlobalConstants;
    using static LetsSport.Web.Common.ConfirmationMessages;

    [Authorize]
    public class EventsController : BaseController
    {
        private const int ItemsPerPage = 8;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly IArenasService arenasService;
        private readonly IEventsService eventsService;
        private readonly IMessagesService messagesService;
        private readonly ISportsService sportsService;
        private readonly IRentalRequestsService rentalRequestsService;
        private readonly UserManager<ApplicationUser> userManager;

        public EventsController(
            ICitiesService citiesService,
            ICountriesService countriesService,
            IArenasService arenasService,
            IUsersService usersService,
            IEventsService eventsService,
            IMessagesService messagesService,
            ISportsService sportsService,
            IRentalRequestsService rentalRequestsService,
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
            this.rentalRequestsService = rentalRequestsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            this.SetLocation();
            var userId = this.userManager.GetUserId(this.User);
            var countryName = this.GetLocation().Country;
            var countryId = await this.countriesService.GetIdAsync(countryName);

            var administratingEvents = await this.eventsService
                .GetAllAdministratingByUserIdAsync<EventCardPartialViewModel>(countryId, userId, ItemsPerPage);
            var participatingEvents = await this.eventsService
                .GetAllUpcomingByUserIdAsync<EventCardPartialViewModel>(countryId, userId, ItemsPerPage);
            var canceledEvents = await this.eventsService
                .GetAdminAllCanceledAsync<EventCardPartialViewModel>(userId, ItemsPerPage);

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
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);

            var viewModel = new EventCreateInputModel
            {
                Arenas = await this.arenasService.GetAllActiveInCitySelectListAsync(cityId),
                Sports = await this.sportsService.GetAllInCityByIdAsync(cityId),
                Date = DateTime.UtcNow,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateInputModel inputModel)
        {
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);

            if (!this.ModelState.IsValid)
            {
                inputModel.Arenas = await this.arenasService.GetAllActiveInCitySelectListAsync(cityId);
                inputModel.Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId);
                inputModel.Date = DateTime.UtcNow;

                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var id = await this.eventsService.CreateAsync(
                inputModel, cityId, countryId, user.Id, user.Email, user.UserName);
            this.TempData[TempDataMessage] = EventCreated;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var viewModel = await this.eventsService.GetDetailsAsync(id, userId);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var userIsAdmin = await this.eventsService.IsUserAdminOnEvent(userId, id);

            if (userIsAdmin == false)
            {
                return new ForbidResult();
            }

            var inputModel = await this.eventsService.GetDetailsForEditAsync(id);

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
                var inputModel = await this.eventsService.GetDetailsForEditAsync(viewModel.Id);

                return this.View(inputModel);
            }

            await this.eventsService.UpdateAsync(viewModel);
            var id = viewModel.Id;
            this.TempData[TempDataMessage] = EventUpdated;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> AddUser(int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.AddUserAsync(id, user.Id, user.Email, user.UserName);
            this.TempData[TempDataMessage] = JoinedEvent;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> RemoveUser(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.RemoveUserAsync(id, user.Id, user.UserName, user.Email);
            this.TempData[TempDataMessage] = LeftEvent;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.eventsService.CancelEventAsync(id, user.Email, user.UserName);
            this.TempData[TempDataMessage] = CancelEvent;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> Invite(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var invitedUsersCount = await this.eventsService.InviteUsersToEventAsync(id, user.Email, user.UserName);
            this.TempData[TempDataMessage] = string.Format(InvitedUserCount, invitedUsersCount);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        public async Task<IActionResult> SendRequest(int id, int arenaId)
        {
            await this.rentalRequestsService.CreateAsync(id, arenaId);
            await this.eventsService.SetSentRequestStatus(id);
            this.TempData[TempDataMessage] = RequestSent;

            return this.RedirectToAction(nameof(this.Details), new { id });
        }
    }
}
