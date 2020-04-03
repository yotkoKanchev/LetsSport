namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class CitiesController : Controller
    {
        private readonly ICountriesService countriesService;
        private readonly ICitiesService citiesService;

        public CitiesController(ICountriesService countriesService, ICitiesService citiesService)
        {
            this.countriesService = countriesService;
            this.citiesService = citiesService;
        }

        public IActionResult Country()
        {
            var viewModel = new ChooseCountryInputModel
            {
                Countries = this.countriesService.GetAll(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Country(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = this.countriesService.GetCountryNameById(countryId),
                Cities = this.citiesService.GetAll<InfoViewModel>(countryId),
            };

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Index(int countryId)
        {
            var countryName = this.countriesService.GetCountryNameById(countryId);

            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = countryName,
                Cities = this.citiesService.GetAll<InfoViewModel>(countryId),
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(int countryId, int isDeleted)
        {
            var viewModel = this.citiesService.FilterCities(countryId, isDeleted);

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Create(int countryId)
        {
            var viewModel = new CreateInputModel
            {
                CountryName = this.countriesService.GetCountryNameById(countryId),
                CountryId = countryId,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.citiesService.CreateCityAsync(inputModel.Name, inputModel.CountryId);
            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.citiesService.GetCityById<EditViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.Countries = this.countriesService.GetAll();

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
                inputModel.Countries = this.countriesService.GetAll();
                return this.View(inputModel);
            }

            await this.citiesService.UpdateCityAsync(inputModel.Id, inputModel.Name, inputModel.CountryId, inputModel.IsDeleted);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.citiesService.GetCityById<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.citiesService.DeleteById(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
