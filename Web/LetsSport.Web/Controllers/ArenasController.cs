namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;

        public ArenasController(IArenasService arenasService, ICitiesService citiesService, ICountriesService countriesService)
        {
            this.arenasService = arenasService;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
        }

        public async Task<IActionResult> Create()
        {
            // TODO pass filtered cities per country
            // TODO add current country as default
            var currentCity = this.HttpContext.Session.GetString("city");
            var currentCountry = this.HttpContext.Session.GetString("country");
            var countries = this.countriesService.GetAll();
            this.ViewData["countries"] = countries;
            var cities = await this.citiesService.GetCitiesAsync(currentCity, currentCountry);
            this.ViewData["cities"] = cities;
            this.ViewData["city"] = currentCity;
            this.ViewData["country"] = currentCountry;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var currentCity = this.HttpContext.Session.GetString("city");
                var currentCountry = this.HttpContext.Session.GetString("country");
                var cities = await this.citiesService.GetCitiesAsync(currentCity, currentCountry);
                this.ViewData["cities"] = cities;

                return this.View(inputModel);
            }

            var arenaId = await this.arenasService.CreateAsync(inputModel);

            // TODO pass filtered by sport Arenas with AJAX;
            return this.Redirect($"details/{arenaId}");
        }

        public IActionResult Details(int id)
        {
            var inputModel = this.arenasService.GetDetails(id);

            return this.View(inputModel);
        }

        public IActionResult Edit(int id)
        {
            var inputModel = this.arenasService.GetArenaForEdit(id);

            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArenaEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                var inputModel = this.arenasService.GetArenaForEdit(viewModel.Id);

                return this.View(inputModel);
            }

            await this.arenasService.UpdateArenaAsync(viewModel);

            var arenaId = viewModel.Id;
            return this.Redirect($"/arenas/details/{arenaId}");
        }
    }
}
