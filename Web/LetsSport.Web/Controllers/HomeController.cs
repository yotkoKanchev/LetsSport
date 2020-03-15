namespace LetsSport.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;

        public HomeController(ILocationLocator locator, IEventsService eventsService, IUsersService usersService, ICitiesService citiesService)
            : base(locator)
        {
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.citiesService = citiesService;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction(nameof(this.IndexLoggedIn));
            }

            this.SetLocation();
            var location = this.GetLocation();

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetAll<HomeEventInfoViewModel>(location),
                Cities = await this.citiesService.GetCitiesWhitEventsAsync(location),
                Sports = this.eventsService.GetAllSportsInCurrentCountry(location.Country),
                Sport = "sport",
                City = "city",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow.AddMonths(6),
            };

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> IndexLoggedIn(/*int? pageNumber*/)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            this.SetLocation();
            var location = this.GetLocation();

            var participatingEvents = await this.eventsService.GetParticipatingEvents<HomeEventInfoViewModel>(userId, location, 5);
            var notParticipatingEvents = await this.eventsService.GetNotParticipatingEvents<HomeEventInfoViewModel>(userId, location, 15);

            var viewModel = new HomeIndexLoggedEventsListViewModel
            {
                ParticipatingEvents = participatingEvents,
                NotParticipatingEvents = notParticipatingEvents,
            };

            // 3. var passedEvents = this.eventsService.GetAllPassedEventsById(userId);
            // 4. Update View

            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Filter(EventsFilterInputModel inputModel)
        {
            this.ViewData["location"] = this.HttpContext.Session.GetString("location");
            var location = this.GetLocation();

            var viewModel = this.eventsService.FilterEventsAsync(inputModel, location);

            return this.View("index", viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode == Common.StatusCodes.NotFound)
            {
                return this.Redirect($"/Error/{Common.StatusCodes.NotFound}");
            }

            return this.Redirect($"/Error/{Common.StatusCodes.InternalServerError}");
        }
    }
}
