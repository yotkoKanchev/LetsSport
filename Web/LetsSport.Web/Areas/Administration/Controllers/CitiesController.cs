namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Services.Data.Cities;
    using LetsSport.Services.Data.Countries;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
    public class CitiesController : Controller
    {
        private const int ItemsPerPage = GlobalConstants.AdminItemsPerPageCount;
        private readonly ICountriesService countriesService;
        private readonly ICitiesService citiesService;

        public CitiesController(ICountriesService countriesService, ICitiesService citiesService)
        {
            this.countriesService = countriesService;
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
                Cities = await this.citiesService.GetAllByCountryIdAsync<CityInfoViewModel>(countryId, ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.citiesService.GetCountInCountryAsync(countryId);
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 1;

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(int countryId, int deletionStatus, int page = 1)
        {
            var viewModel = await this.citiesService.FilterAsync(countryId, deletionStatus, ItemsPerPage, (page - 1) * ItemsPerPage);
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

        public async Task<IActionResult> Create(int countryId)
        {
            var viewModel = new CreateInputModel
            {
                CountryName = await this.countriesService.GetNameByIdAsync(countryId),
                CountryId = countryId,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.citiesService.CreateAsync(inputModel.Name, inputModel.CountryId);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.citiesService.GetByIdAsync<EditViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.Countries = await this.countriesService.GetAllAsSelectListAsync();

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
                inputModel.Countries = await this.countriesService.GetAllAsSelectListAsync();
                return this.View(inputModel);
            }

            await this.citiesService.UpdateAsync(inputModel.Id, inputModel.Name, inputModel.CountryId, inputModel.IsDeleted);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.citiesService.GetByIdAsync<DeleteViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Archive(int id, int countryId)
        {
            await this.citiesService.ArchiveByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.citiesService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
