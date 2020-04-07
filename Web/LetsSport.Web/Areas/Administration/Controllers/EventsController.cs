﻿namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Events;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class EventsController : Controller
    {
        private const int ItemsPerPage = 10;
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

        public async Task<IActionResult> Country()
        {
            var viewModel = new ChooseCountryInputModel
            {
                Countries = await this.countriesService.GetAllAsSelectListAsync(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Country(int countryId)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Events = await this.eventsService.GetAllInCountryAsync<InfoViewModel>(countryId),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return this.RedirectToAction(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Index(int countryId, int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Events = await this.eventsService.GetAllInCountryAsync<InfoViewModel>(countryId, ItemsPerPage, (page - 1) * ItemsPerPage),
                Filter = new FilterBarViewModel
                {
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            var count = await this.eventsService.GetCountInCountryAsync(countryId);

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

        public async Task<IActionResult> Filter(FilterBarViewModel inputModel, int page = 1)
        {
            var viewModel = await this.eventsService.AdminFilterAsync(
                inputModel.CountryId,
                inputModel.CityId,
                inputModel.SportId,
                inputModel.IsDeleted,
                ItemsPerPage,
                (page - 1) * ItemsPerPage);

            var count = viewModel.ResultCount;

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

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var arena = await this.eventsService.GetEventByIdAsync<DetailsViewModel>(id.Value);

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

            var viewModel = await this.eventsService.GetEventByIdAsync<EditViewModel>(id.Value);
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.eventsService.GetEventByIdAsync<DeleteViewModel>(id.Value);

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
