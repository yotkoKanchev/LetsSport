﻿namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class CitiesController : Controller
    {
        private const int ItemsPerPage = 12;
        private readonly ICountriesService countriesService;
        private readonly ICitiesService citiesService;

        public CitiesController(ICountriesService countriesService, ICitiesService citiesService)
        {
            this.countriesService = countriesService;
            this.citiesService = citiesService;
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
                Cities = await this.citiesService.GetAllByCountryIdAsync<InfoViewModel>(countryId),
            };

            return this.RedirectToAction(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Index(int countryId, int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Location = await this.countriesService.GetNameByIdAsync(countryId),
                Cities = await this.citiesService.GetAllByCountryIdAsync<InfoViewModel>(countryId, ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.citiesService.GetCountInCountryAsync(countryId);

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

        public async Task<IActionResult> Filter(int countryId, int deletionStatus, int page = 1)
        {
            var viewModel = await this.citiesService.FilterAsync(countryId, deletionStatus, ItemsPerPage, (page - 1) * ItemsPerPage);

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

        public async Task<IActionResult> Create(int countryId)
        {
            var viewModel = new CreateInputModel
            {
                CountryName = await this.countriesService.GetNameByIdAsync(countryId),
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

            await this.citiesService.CreateAsync(inputModel.Name, inputModel.CountryId);
            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.citiesService.GetByIdAsync<EditViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.Countries = await this.countriesService.GetAllAsSelectListAsync();

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
                inputModel.Countries = await this.countriesService.GetAllAsSelectListAsync();
                return this.View(inputModel);
            }

            await this.citiesService.UpdateAsync(inputModel.Id, inputModel.Name, inputModel.CountryId, inputModel.IsDeleted);

            return this.RedirectToAction(nameof(this.Index), new { countryId = inputModel.CountryId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var viewModel = await this.citiesService.GetByIdAsync<DeleteViewModel>(id.Value);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id, int countryId)
        {
            await this.citiesService.ArchiveByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int countryId)
        {
            await this.citiesService.DeleteByIdAsync(id);

            return this.RedirectToAction(nameof(this.Index), new { countryId });
        }
    }
}
