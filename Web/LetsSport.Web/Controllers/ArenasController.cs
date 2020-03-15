namespace LetsSport.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;

        public ArenasController(
            IArenasService arenasService,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.arenasService = arenasService;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
        }

        public async Task<IActionResult> Create()
        {
            this.SetLocation();
            var location = this.GetLocation();

            var viewModel = new ArenaCreateInputModel
            {
                Sports = this.sportsService.GetAll(),
                Countries = this.countriesService.GetAll(),
                Cities = await this.citiesService.GetCitiesAsync(location),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                inputModel.Sports = this.sportsService.GetAll();
                inputModel.Countries = this.countriesService.GetAll();
                inputModel.Cities = await this.citiesService.GetCitiesAsync(location);

                return this.View(inputModel);
            }

            var arenaId = await this.arenasService.CreateAsync(inputModel);

            // TODO pass filtered by sport Arenas with AJAX;
            return this.Redirect($"details/{arenaId}");
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var viewModel = this.arenasService.GetDetails(id);
            viewModel.LoggedUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return this.View(viewModel);
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
