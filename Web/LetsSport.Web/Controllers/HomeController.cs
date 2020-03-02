namespace LetsSport.Web.Controllers
{
    using System.Diagnostics;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ILocationLocator locator;
        private readonly IEventsService eventsService;
        private readonly IUsersService usersService;
        private readonly string currentLocation;

        public HomeController(ILocationLocator locator, IEventsService eventsService, IUsersService usersService)
        {
            this.locator = locator;
            this.eventsService = eventsService;
            this.usersService = usersService;
            var currentLocation = this.locator.GetLocationInfo();
            this.currentLocation = currentLocation.City + ", " + currentLocation.Country;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            //if (this.User.Identity.IsAuthenticated)
            //{
            //    return this.RedirectToAction(nameof(this.IndexLoggedIn));
            //}

            this.ViewData["location"] = this.currentLocation;

            var viewModel = this.eventsService.GetAll();
            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Filter(EventsFilterInputModel inputModel)
        {
            this.ViewData["location"] = this.currentLocation;
            var viewModel = this.eventsService.FilterEventsAsync(inputModel);

            return this.View("index", viewModel);
        }

        //[Authorize]
        //[HttpGet]
        //[Route("/Home/Index")]
        //public async Task<IActionResult> IndexLoggedIn(int? pageNumber)
        //{
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
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
