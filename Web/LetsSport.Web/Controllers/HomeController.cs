namespace LetsSport.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private const int ItemsPerPage = 8;
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
            await this.eventsService.SetPassedStatusAsync(countryId);

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetAllInCityAsync<EventCardPartialViewModel>(
                    cityId, ItemsPerPage, (page - 1) * ItemsPerPage),
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
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PageCount == 0)
            {
                viewModel.PageCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> IndexLoggedIn(int page = 1)
        {
            this.SetLocation();
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);
            await this.eventsService.SetPassedStatusAsync(countryId);
            var userId = this.userManager.GetUserId(this.User);

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetNotParticipatingInCityAsync<EventCardPartialViewModel>(
                    userId, cityId, ItemsPerPage, (page - 1) * ItemsPerPage),
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
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PageCount == 0)
            {
                viewModel.PageCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(
            int? cityId, int? sportId, DateTime from, DateTime to, int page = 1)
        {
            this.SetLocation();
            var countryName = this.GetLocation().Country;
            var countryId = await this.countriesService.GetIdAsync(countryName);
            await this.eventsService.SetPassedStatusAsync(countryId);
            var userId = this.userManager.GetUserId(this.User);

            var viewModel = await this.eventsService.FilterEventsAsync(
                cityId, sportId, from, to, countryId, userId, ItemsPerPage, (page - 1) * ItemsPerPage);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var count = viewModel.ResultCount;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage);
            viewModel.CurrentPage = page;

            if (viewModel.PageCount == 0)
            {
                viewModel.PageCount = 1;
            }

            viewModel.Location = cityId.HasValue
                ? await this.citiesService.GetNameByIdAsync(cityId.Value) + ", " + countryName
                : $"{countryName}";

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
