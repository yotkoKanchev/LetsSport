namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Sports;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class SportsController : Controller
    {
        private readonly ISportsService sportsService;

        public SportsController(ISportsService sportsService)
        {
            this.sportsService = sportsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexListViewModel
            {
                Sports = this.sportsService.GetAll<InfoViewModel>(),
            };

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

            var id = await this.sportsService.CreateSport(inputModel.Name, inputModel.Image);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.sportsService.GetSportById<EditViewModel>(id.Value);

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

            await this.sportsService.UpdateSport(inputModel.Id, inputModel.Name, inputModel.Image);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.sportsService.GetSportById<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await this.sportsService.DeleteById(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
