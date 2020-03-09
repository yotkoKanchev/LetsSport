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
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            var countries = this.countriesService.GetAll();
            this.ViewData["countries"] = countries;
            var cities = await this.citiesService.GetCitiesAsync(ip);
            this.ViewData["cities"] = cities;
            this.ViewData["city"] = this.HttpContext.Session.GetString("city");
            this.ViewData["country"] = this.HttpContext.Session.GetString("country");
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

                var cities = await this.citiesService.GetCitiesAsync(ip);
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
