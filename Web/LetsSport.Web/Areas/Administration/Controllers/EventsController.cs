namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Events;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class EventsController : Controller
    {
        private readonly IEventsService eventsService;
        private readonly ICountriesService countriesService;
        private readonly ICitiesService citiesService;
        private readonly ISportsService sportsService;
        private readonly IArenasService arenasService;

        public EventsController(
            IEventsService eventsService,
            ICountriesService countriesService,
            ICitiesService citiesService,
            ISportsService sportsService,
            IArenasService arenasService)
        {
            this.eventsService = eventsService;
            this.countriesService = countriesService;
            this.citiesService = citiesService;
            this.sportsService = sportsService;
            this.arenasService = arenasService;
        }

        public IActionResult Country()
        {
            var viewModel = new ChooseCountryInputModel
            {
                Countries = this.countriesService.GetAllAsSelectList(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Country(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                Location = this.countriesService.GetNameById(countryId),
                Events = await this.eventsService.GetAllInCountryAsync<InfoViewModel>(countryId),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Index(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                Location = this.countriesService.GetNameById(countryId),
                Events = await this.eventsService.GetAllInCountryAsync<InfoViewModel>(countryId),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(FilterBarViewModel inputModel)
        {
            var viewModel = this.eventsService.FilterAsync(inputModel.CountryId, inputModel.CityId, inputModel.SportId);

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var arena = this.eventsService.GetEventById<DetailsViewModel>(id.Value);

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

            var viewModel = this.eventsService.GetEventById<EditViewModel>(id.Value);
            viewModel.Sports = this.sportsService.GetAllAsSelectList();
            viewModel.Arenas = await this.arenasService.GetAllInCitySelectListAsync(viewModel.CityId);

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

            await this.eventsService.AdminUpdateAsync(inputModel);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.eventsService.GetEventById<DeleteViewModel>(id.Value);

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.eventsService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
