namespace LetsSport.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IUsersService usersService;
        private readonly IImagesService imagesService;
        private readonly UserManager<ApplicationUser> userManager;

        public ArenasController(
            IArenasService arenasService,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IUsersService usersService,
            ILocationLocator locationLocator,
            IImagesService imagesService,
            UserManager<ApplicationUser> userManager)
            : base(locationLocator)
        {
            this.arenasService = arenasService;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.usersService = usersService;
            this.imagesService = imagesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Create()
        {
            this.SetLocation();
            var location = this.GetLocation();

            var viewModel = new ArenaCreateInputModel
            {
                Sports = this.sportsService.GetAll(),
                Countries = this.countriesService.GetAll(),
                Cities = await this.citiesService.GetCitiesAsync(location),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                inputModel.Sports = this.sportsService.GetAll();
                inputModel.Countries = this.countriesService.GetAll();
                inputModel.Cities = await this.citiesService.GetCitiesAsync(location);

                return this.View(inputModel);
            }

            var userId = this.userManager.GetUserId(this.User);
            var id = await this.arenasService.CreateAsync(inputModel, userId);

            // TODO pass filtered by sport Arenas with AJAX;
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var viewModel = this.arenasService.GetDetails(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.LoggedUserId = this.userManager.GetUserId(this.User);
            return this.View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var inputModel = this.arenasService.GetArenaForEdit(id);

            if (userId != inputModel.ArenaAdminId)
            {
                return new ForbidResult();
            }

            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArenaEditViewModel viewModel)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (userId != viewModel.ArenaAdminId)
            {
                return new ForbidResult();
            }

            if (!this.ModelState.IsValid)
            {
                var inputModel = this.arenasService.GetArenaForEdit(viewModel.Id);

                return this.View(inputModel);
            }

            await this.arenasService.UpdateArenaAsync(viewModel);

            var id = viewModel.Id;
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMainImage(ArenaDetailsViewModel viewModel, int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaAdminId = this.usersService.GetArenaAdminIdByArenaId(id);
            if (userId != arenaAdminId)
            {
                return new ForbidResult();
            }

            if (viewModel.NewMainImage == null)
            {
                return this.NoContent();
            }

            await this.arenasService.ChangeMainImageAsync(id, viewModel.NewMainImage);
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMainImage(int id)
        {
            var userId = this.userManager.GetUserId(this.User);

            var arenaAdminId = this.usersService.GetArenaAdminIdByArenaId(id);
            if (userId != arenaAdminId)
            {
                return new ForbidResult();
            }

            await this.arenasService.DeleteMainImage(id);
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaAdminId = this.imagesService.GetArenaAdminIdByImageId(id);
            var arnaId = this.arenasService.GetArenaIdByAdminId(arenaAdminId);

            if (userId != arenaAdminId)
            {
                return new ForbidResult();
            }

            await this.imagesService.DeleteImageAsync(id);
            return this.RedirectToAction(nameof(this.EditImages), new { arnaId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImages(string[] ids, string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaAdminId = this.imagesService.GetArenaAdminIdByImageId(id);
            var arnaId = this.arenasService.GetArenaIdByAdminId(arenaAdminId);

            if (userId != arenaAdminId)
            {
                return new ForbidResult();
            }

            foreach (var imageId in ids)
            {
                await this.imagesService.DeleteImageAsync(id);
            }

            return this.RedirectToAction(nameof(this.EditImages), new { arnaId });
        }

        public IActionResult EditImages(int id)
        {
            var viewModel = this.arenasService.GetArenasImagesByArenaId(id);

            return this.View(viewModel);
        }
    }
}
