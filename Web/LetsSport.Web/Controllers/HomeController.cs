namespace LetsSport.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.GlobalConstants;

    public class HomeController : BaseController
    {
        private readonly ICountriesService countriesService;
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;
        private readonly ISportsService sportsService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(
            ILocationLocator locator,
            ICountriesService countriesService,
            IEventsService eventsService,
            IUsersService usersService,
            ICitiesService citiesService,
            ISportsService sportsService,
            UserManager<ApplicationUser> userManager)
            : base(locator)
        {
            this.countriesService = countriesService;
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.citiesService = citiesService;
            this.sportsService = sportsService;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction(nameof(this.IndexLoggedIn));
            }

            this.SetLocation();
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetAllInCityAsync<EventCardPartialViewModel>(
                    countryId, cityId, ResultsPerPageCount, (page - 1) * ResultsPerPageCount),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithEventsInCountryAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                },
                Location = location.City + ", " + location.Country,
            };

            var count = await this.eventsService.GetCountInCityAsync(cityId);
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ResultsPerPageCount) != 0
                ? (int)Math.Ceiling((double)count / ResultsPerPageCount) : 1;

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> IndexLoggedIn(int page = 1)
        {
            this.SetLocation();
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);
            var userId = this.userManager.GetUserId(this.User);

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetNotParticipatingInCityAsync<EventCardPartialViewModel>(
                    countryId, userId, cityId, ResultsPerPageCount, (page - 1) * ResultsPerPageCount),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithEventsInCountryAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                    From = DateTime.UtcNow,
                    To = DateTime.UtcNow.AddMonths(6),
                },
                Location = location.City + ", " + location.Country,
            };

            var count = await this.eventsService.GetNotParticipatingCount(userId, cityId);
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ResultsPerPageCount) != 0
                ? (int)Math.Ceiling((double)count / ResultsPerPageCount) : 1;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(
            int? cityId, int? sportId, DateTime from, DateTime to, int page = 1)
        {
            this.SetLocation();
            var countryName = this.GetLocation().Country;
            var countryId = await this.countriesService.GetIdAsync(countryName);
            var userId = this.userManager.GetUserId(this.User);

            var viewModel = await this.eventsService.FilterAsync(
                cityId, sportId, from, to, countryId, userId, ResultsPerPageCount, (page - 1) * ResultsPerPageCount);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var count = viewModel.ResultCount;
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ResultsPerPageCount) != 0
                ? (int)Math.Ceiling((double)count / ResultsPerPageCount) : 1;

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
            if (statusCode == StatusCodes.NotFound)
            {
                return this.Redirect($"/Error/{StatusCodes.NotFound}");
            }

            return this.Redirect($"/Error/{StatusCodes.InternalServerError}");
        }
    }
}
