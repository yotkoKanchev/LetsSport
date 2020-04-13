namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Sports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
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

            var count = await this.sportsService.GetCountAsync();
            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

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

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.sportsService.GetByIdAsync<EditViewModel>(id.Value);

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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.sportsService.GetByIdAsync<DeleteViewModel>(id.Value);

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
