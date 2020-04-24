namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
    public class ArenasController : Controller
    {
        private const int ItemsPerPage = GlobalConstants.AdminItemsPerPageCount;
        private readonly IArenasService arenasService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        public ArenasController(
            IArenasService arenasService,
            ICountriesService countriesService,
            ISportsService sportsService,
            ICitiesService citiesService)
        {
            this.arenasService = arenasService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
        }

        public async Task<IActionResult> Country()
        {
            var viewModel = new ChooseCountryInputModel
            {
                Countries = await this.countriesService.GetAllAsSelectListAsync(),
            };

            return this.View(viewModel);
        }


        public async Task<IActionResult> Index(int countryId, int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Arenas = await this.arenasService.GetAllInCountryAsync<InfoViewModel>(
                    countryId, ItemsPerPage, (page - 1) * ItemsPerPage),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            var count = await this.arenasService.GetCountInCountryAsync(countryId);
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 1;

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(FilterBarViewModel inputModel, int page = 1)
        {
            var viewModel = await this.arenasService.AdminFilterAsync(
                inputModel.CountryId,
                inputModel.CityId,
                inputModel.SportId,
                ItemsPerPage,
                (page - 1) * ItemsPerPage);

            var count = viewModel.ResultCount;
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 1;

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var arena = await this.arenasService.GetByIdAsync<DetailsViewModel>(id.Value);

            if (arena == null)
            {
                return this.NotFound();
            }

            return this.View(arena);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.arenasService.GetByIdAsync<EditViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditViewModel inputModel)
        {
            if (id != inputModel.Id)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.arenasService.AdminUpdateAsync(inputModel);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }
    }
}
