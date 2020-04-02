namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class ArenasController : Controller
    {
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

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                Arenas = this.arenasService.GetAll<InfoViewModel>(),
                Filter = new FilterBarViewModel
                {
                    Countries = this.countriesService.GetAll(),
                },
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(FilterBarViewModel inputModel)
        {
            IndexViewModel viewModel;

            if (inputModel.City == null && inputModel.Sport == null && inputModel.IsDeleted == null)
            {
                viewModel = this.arenasService.FilterArenasByCountryId(inputModel.Country);
            }
            else
            {
                viewModel = this.arenasService.FilterArenas(inputModel.Country, inputModel.City, inputModel.Sport, inputModel.IsDeleted);
            }

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var arena = this.arenasService.GetArenaById<DetailsViewModel>(id.Value);

            if (arena == null)
            {
                return this.NotFound();
            }

            return this.View(arena);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.arenasService.GetArenaById<EditViewModel>(id.Value);
            viewModel.Sports = this.sportsService.GetAll();

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

            await this.arenasService.AdminUpdateArenaAsync(inputModel);

            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.arenasService.GetArenaById<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await this.arenasService.DeleteById(id);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
