namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class ArenasController : Controller
    {
        private const int ItemsPerPage = 20;
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

        [HttpPost]
        public async Task<IActionResult> Country(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Arenas = await this.arenasService.GetAllInCountryAsync<InfoViewModel>(countryId),
                Filter = new FilterBarViewModel
                {
                    CountryId = countryId,
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return this.RedirectToAction(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Index(int countryId, int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Arenas = await this.arenasService.GetAllInCountryAsync<InfoViewModel>(countryId, ItemsPerPage, (page - 1) * ItemsPerPage),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            var count = await this.arenasService.GetCountInCountryAsync(countryId);

            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

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
                inputModel.IsDeleted,
                ItemsPerPage,
                (page - 1) * ItemsPerPage);

            var count = viewModel.ResultCount;

            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var arena = this.arenasService.GetByIdAsync<DetailsViewModel>(id.Value);

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
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.arenasService.GetByIdAsync<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.arenasService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
