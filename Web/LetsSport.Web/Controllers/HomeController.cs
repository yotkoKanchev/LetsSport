namespace LetsSport.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private const int EventsPerPage = 12;
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;
        private readonly ISportsService sportsService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(
            ILocationLocator locator,
            IEventsService eventsService,
            IUsersService usersService,
            ICitiesService citiesService,
            ISportsService sportsService,
            UserManager<ApplicationUser> userManager)
            : base(locator)
        {
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.citiesService = citiesService;
            this.sportsService = sportsService;
            this.userManager = userManager;
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
                Events = await this.eventsService.GetAllInCity<EventCardPartialViewModel>(location, EventsPerPage),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(location.Country),
                    Sports = this.sportsService.GetAllSportsInCountry(location.Country),
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                },
            };

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> IndexLoggedIn(/*int? pageNumber*/)
        {
            this.SetLocation();
            var userId = this.userManager.GetUserId(this.User);
            var location = this.GetLocation();

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetNotParticipatingEventsInCity<EventCardPartialViewModel>(userId, location, EventsPerPage),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(location.Country),
                    Sports = this.sportsService.GetAllSportsInCountry(location.Country),
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                },
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(int city, int sport, DateTime from, DateTime to)
        {
            this.SetLocation();
            var country = this.GetLocation().Country;
            var userId = this.userManager.GetUserId(this.User);
            var viewModel = await this.eventsService.FilterEventsAsync(city, sport, from, to, country, userId);

            this.ViewData["location"] = city == 0
                ? country
                : this.citiesService.GetLocationByCityId(city);

            if (this.User.Identity.IsAuthenticated)
            {
                return this.View(nameof(this.IndexLoggedIn), viewModel);
            }

            return this.View(nameof(this.Index), viewModel);
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
