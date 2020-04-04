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
                Countries = this.countriesService.GetAll(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Country(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                Location = this.countriesService.GetCountryNameById(countryId),
                Events = this.eventsService.GetAllInCountry<InfoViewModel>(countryId).ToList(),
                Filter = new FilterBarViewModel
                {
                    Cities = this.citiesService.GetCitiesInCountryById(countryId).ToList(),
                    Sports = this.sportsService.GetAllSportsInCountryById(countryId).ToList(),
                },
            };

            return this.View(nameof(this.Index), viewModel);
        }

        public IActionResult Index(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                Location = this.countriesService.GetCountryNameById(countryId),
                Events = this.eventsService.GetAllInCountry<InfoViewModel>(countryId).ToList(),
                Filter = new FilterBarViewModel
                {
                    Cities = this.citiesService.GetCitiesInCountryById(countryId).ToList(),
                    Sports = this.sportsService.GetAllSportsInCountryById(countryId).ToList(),
                },
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(FilterBarViewModel inputModel)
        {
            var viewModel = this.eventsService.FilterEvents(inputModel.CountryId, inputModel.City, inputModel.Sport);

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

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = this.eventsService.GetEventById<EditViewModel>(id.Value);
            viewModel.Sports = this.sportsService.GetAll();
            viewModel.Arenas = this.arenasService.GetAllArenasInCitySelectList(viewModel.CityId);

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

            await this.eventsService.AdminUpdateEventAsync(inputModel);

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
            await this.eventsService.DeleteById(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
