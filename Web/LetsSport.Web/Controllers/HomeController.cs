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
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(
            ILocationLocator locator,
            IEventsService eventsService,
            IUsersService usersService,
            ICitiesService citiesService,
            UserManager<ApplicationUser> userManager)
            : base(locator)
        {
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.citiesService = citiesService;
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
            var country = this.GetLocation().Country;

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetAll<HomeEventInfoViewModel>(country),
                Partial = new _FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                    Sports = this.eventsService.GetAllSportsInCurrentCountry(country),
                    City = "city",
                    Sport = "sport",
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                    Controller = "Home",
                    Action = "Filter",
                },
            };

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> IndexLoggedIn(/*int? pageNumber*/)
        {
            var userId = this.userManager.GetUserId(this.User);
            this.SetLocation();
            var country = this.GetLocation().Country;

            var notParticipatingEvents = await this.eventsService.GetNotParticipatingEvents<HomeEventInfoViewModel>(userId, country, 12);

            // TODO filter only in notParticipating
            var viewModel = new HomeIndexLoggedEventsListViewModel
            {
                NotParticipatingEvents = notParticipatingEvents,
                Partial = new _FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                    Sports = this.eventsService.GetAllSportsInCurrentCountry(country),
                    City = "city",
                    Sport = "sport",
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                    Controller = "Home",
                    Action = "FilterLogged",
                },
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(string city, string sport, DateTime from, DateTime to)
        {
            this.SetLocation();
            var location = this.GetLocation();
            var viewModel = await this.eventsService.FilterEventsAsync(city, sport, from, to, location.Country);
            this.ViewData["location"] = city == "city"
                ? location.City + ", " + location.Country
                : city + ", " + viewModel.Partial.Country;

            return this.View(nameof(this.Index), viewModel);
        }

        [Authorize]
        public async Task<IActionResult> FilterLogged(string city, string sport, DateTime from, DateTime to)
        {
            this.SetLocation();
            var country = this.GetLocation().Country;
            var userId = this.userManager.GetUserId(this.User);
            var viewModel = await this.eventsService.FilterEventsLoggedAsync(city, sport, from, to, userId, country);

            return this.View(nameof(this.IndexLoggedIn), viewModel);
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
