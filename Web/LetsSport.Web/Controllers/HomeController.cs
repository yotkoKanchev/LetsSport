namespace LetsSport.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ILocationLocator locator;
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly ICitiesService citiesService;

        public HomeController(ILocationLocator locator, IEventsService eventsService, IUsersService usersService, ICitiesService citiesService)
        {
            this.locator = locator;
            this.eventsService = eventsService;
            this.usersService = usersService;
            this.citiesService = citiesService;
        }

        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // if (this.User.Identity.IsAuthenticated)
            // {
            //    return this.RedirectToAction(nameof(this.IndexLoggedIn));
            // }
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            string currentCity;
            string currentCountry;

            if (this.HttpContext.Session.GetString("city") != null)
            {
                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
                currentCity = this.HttpContext.Session.GetString("city");
                currentCountry = this.HttpContext.Session.GetString("country");
            }
            else
            {
                var currentLocation = this.locator.GetLocationInfo(ip);
                currentCity = currentLocation.City;
                currentCountry = currentLocation.Country;
                var location = currentLocation.City + ", " + currentLocation.Country;

                this.HttpContext.Session.SetString("city", currentCity);
                this.HttpContext.Session.SetString("country", currentCountry);
                this.HttpContext.Session.SetString("location", location);

                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
            }

            var viewModel = new HomeEventsListViewModel
            {
                Events = await this.eventsService.GetAll<HomeEventInfoViewModel>(currentCity, currentCountry),
                Cities = await this.citiesService.GetCitiesWhitEventsAsync(currentCity, currentCountry),
                Sports = this.eventsService.GetAllSportsInCurrentCountry(currentCountry),
                Sport = "sport",
                City = "city",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow.AddMonths(6),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Filter(EventsFilterInputModel inputModel)
        {
            this.ViewData["location"] = this.HttpContext.Session.GetString("location");
            var currentCity = this.HttpContext.Session.GetString("city");
            var currentCountry = this.HttpContext.Session.GetString("country");

            var viewModel = this.eventsService.FilterEventsAsync(inputModel, currentCity, currentCountry);

            return this.View("index", viewModel);
        }

        // [Authorize]
        // [HttpGet]
        // [Route("/Home/Index")]
        // public async Task<IActionResult> IndexLoggedIn(int? pageNumber)
        // {
        //    var currentLocation = this.locator.GetLocationInfo();
        //    var cityName = currentLocation.City;
        //    var countryName = currentLocation.Country;
        //    this.ViewData["location"] = cityName + ", " + countryName;
        //    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    //TODO get all user event from usersService by userId and list them,
        //    var events = this.usersService.GetUserEvents(userId);
        //    var viewModel = new EventsAllDetailsViewModel
        //    {
        //        AllEvents = events.Select(e => new EventInfoViewModel
        //        {
        //            Id = e.Id,
        //            Arena = e.Arena.ToString(),
        //            Sport = e.SportType.ToString(),
        //            Date = e.Date.ToString("dd-MMM-yyyy"),
        //            EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
        //        }),
        //    };
        //    if (events == null)
        //    {
        //        // pass h2 tag with no events joined yet , please join ....
        //        return this.Redirect("/");
        //    }
        //    return this.View(viewModel);
        // }
        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
