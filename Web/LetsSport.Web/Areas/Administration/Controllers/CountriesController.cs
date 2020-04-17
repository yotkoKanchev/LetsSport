namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Countries;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
    public class CountriesController : Controller
    {
        private const int ItemsPerPage = 20;
        private readonly ICountriesService countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            this.countriesService = countriesService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = new IndexListViewModel
            {
                Countries = await this.countriesService.GetAllAsync<CountryInfoViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.countriesService.GetCountAsync();
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 0;

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            var id = await this.countriesService.CreateAsync(inputModel.Name);

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.countriesService.GetByIdAsync<EditViewModel>(id.Value);

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

            await this.countriesService.UpdateAsync(inputModel.Id, inputModel.Name);

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.countriesService.GetByIdAsync<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.countriesService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
