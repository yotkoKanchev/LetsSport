namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Countries;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
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
                Countries = await this.countriesService.GetAllAsync<InfoViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.countriesService.GetCountAsync();
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

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await this.countriesService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
