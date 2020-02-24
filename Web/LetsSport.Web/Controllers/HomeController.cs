namespace LetsSport.Web.Controllers
{
    using System.Diagnostics;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ILocationLocator locator;
        private readonly IEventsService eventsService;

        public HomeController(ILocationLocator locator, IEventsService eventsService)
        {
            this.locator = locator;
            this.eventsService = eventsService;
        }

        public IActionResult Index()
        {
            var currentLocation = this.locator.GetLocationInfo();
            var cityName = currentLocation.City;
            var countryName = currentLocation.Country;
            this.ViewData["location"] = cityName + ", " + countryName;

            var viewModel = this.eventsService.GetAll();
            return this.View(viewModel);
        }

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
