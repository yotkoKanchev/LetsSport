namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Sports;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class SportsController : Controller
    {
        private const int ItemsPerPage = 10;
        private readonly ISportsService sportsService;

        public SportsController(ISportsService sportsService)
        {
            this.sportsService = sportsService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = new IndexListViewModel
            {
                Sports = await this.sportsService.GetAllAsync<InfoViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = this.sportsService.GetCount();
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

            var id = await this.sportsService.AddAsync(inputModel.Name, inputModel.Image);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.sportsService.GetById<EditViewModel>(id.Value);

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

            await this.sportsService.UpdateAsync(inputModel.Id, inputModel.Name, inputModel.Image);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.sportsService.GetById<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await this.sportsService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
