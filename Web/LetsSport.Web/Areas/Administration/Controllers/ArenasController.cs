﻿namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin;
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
                Arenas = this.arenasService.GetAllInCountryAsIQueryable<InfoViewModel>(countryId).ToList(),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllSportsInCountryByIdAsync(countryId),
                },
            };

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Index(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                Location = this.countriesService.GetNameById(countryId),
                Arenas = this.arenasService.GetAllInCountryAsIQueryable<InfoViewModel>(countryId).ToList(),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllSportsInCountryByIdAsync(countryId),
                },
            };

            return this.View(viewModel);
        }

        public IActionResult Filter(FilterBarViewModel inputModel)
        {
            var viewModel = this.arenasService.FilterArenasAsync(inputModel.CountryId, inputModel.City, inputModel.Sport, inputModel.IsDeleted);

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

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
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
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.arenasService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}