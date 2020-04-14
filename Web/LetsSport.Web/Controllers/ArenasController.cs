namespace LetsSport.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Authorize(Roles = GlobalConstants.ArenaAdminRoleName)]
    public class ArenasController : BaseController
    {
        private const int ItemsPerPage = 8;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IArenasService arenasService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IUsersService usersService;
        private readonly IImagesService imagesService;
        private readonly IEventsService eventsService;

        public ArenasController(
            IArenasService arenasService,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IUsersService usersService,
            ILocationLocator locationLocator,
            IImagesService imagesService,
            IEventsService eventsService,
            UserManager<ApplicationUser> userManager)
            : base(locationLocator)
        {
            this.arenasService = arenasService;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.usersService = usersService;
            this.imagesService = imagesService;
            this.eventsService = eventsService;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            this.SetLocation();
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);

            var viewModel = new ArenaIndexListViewModel
            {
                Location = $"{location.City}, {location.Country}",
                Arenas = await this.arenasService.GetAllInCityAsync(cityId, ItemsPerPage, (page - 1) * ItemsPerPage),
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithArenasInCountryAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            var count = await this.arenasService.GetCountInCityAsync(cityId);
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 0;

            return this.View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(int? sportId, int? cityId, int page = 1)
        {
            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);
            var viewModel = await this.arenasService.FilterAsync(
                countryId, sportId, cityId, ItemsPerPage, (page - 1) * ItemsPerPage);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var count = viewModel.ResultCount;
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 0;

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (await this.usersService.IsUserHasArenaAsync(user.Id))
            {
                return this.RedirectToAction(nameof(this.MyArena));
            }

            var location = this.GetLocation();
            var countryId = await this.countriesService.GetIdAsync(location.Country);

            var viewModel = new ArenaCreateInputModel
            {
                Sports = await this.sportsService.GetAllAsSelectListAsync(),
                Countries = await this.countriesService.GetAllAsSelectListAsync(),
                Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                CountryName = location.Country,
                CityName = location.City,
                CountryId = countryId,
                CityId = await this.citiesService.GetIdAsync(location.City, countryId),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                var countryId = await this.countriesService.GetIdAsync(location.Country);
                inputModel.Sports = await this.sportsService.GetAllAsSelectListAsync();
                inputModel.Countries = await this.countriesService.GetAllAsSelectListAsync();
                inputModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId);
                inputModel.CountryId = countryId;
                inputModel.CityId = await this.citiesService.GetIdAsync(location.City, countryId);

                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            await this.arenasService.CreateAsync(inputModel, user.Id, user.Email, user.UserName);
            this.TempData["message"] = $"{inputModel.Name} Arena created successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await this.arenasService.GetDetailsAsync<ArenaDetailsViewModel>(id);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var userId = this.userManager.GetUserId(this.User);

            if (viewModel.ArenaAdminId == userId)
            {
                return this.RedirectToAction(nameof(this.MyArena));
            }

            viewModel.MainImageUrl = this.arenasService.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = await this.arenasService.GetImageUrslByIdAsync(id);

            return this.View(viewModel);
        }

        public async Task<IActionResult> MyArena()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (await this.usersService.IsUserHasArenaAsync(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            var viewModel = await this.arenasService.GetDetailsAsync<MyArenaDetailsViewModel>(arenaId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.MainImageUrl = this.arenasService.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = await this.arenasService.GetImageUrslByIdAsync(arenaId);
            viewModel.LoggedUserId = userId;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Events()
        {
            this.SetLocation();
            var userId = this.userManager.GetUserId(this.User);

            if (await this.usersService.IsUserHasArenaAsync(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var countryName = this.GetLocation().Country;
            var countryId = await this.countriesService.GetIdAsync(countryName);
            var viewModel = await this.eventsService.GetArenaEventsByArenaAdminId(countryId, userId);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            var inputModel = await this.arenasService.GetDetailsForEditAsync(arenaId);

            if (userId != inputModel.ArenaAdminId)
            {
                return new ForbidResult();
            }

            return this.View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArenaEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                var inputModel = await this.arenasService.GetDetailsForEditAsync(viewModel.Id);

                return this.View(inputModel);
            }

            var userId = this.userManager.GetUserId(this.User);
            await this.arenasService.UpdateAsync(viewModel);
            this.TempData["message"] = $"{viewModel.Name} Arena info updated successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMainImage([Bind("NewMainImage")] MyArenaDetailsViewModel viewModel)
        {
            if (viewModel.NewMainImage == null)
            {
                this.TempData["message"] = $"No picture selected!";

                return this.NoContent();
            }

            var userId = this.userManager.GetUserId(this.User);
            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            await this.arenasService.ChangeMainImageAsync(arenaId, viewModel.NewMainImage);
            this.TempData["message"] = $"{viewModel.Name} Arena main image changed successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMainImage()
        {
            if (!this.ModelState.IsValid)
            {
                this.TempData["message"] = $"Error deleting Main image occured!";

                return this.RedirectToAction(nameof(this.MyArena));
            }

            var userId = this.userManager.GetUserId(this.User);
            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            await this.arenasService.DeleteMainImageAsync(arenaId);
            this.TempData["message"] = $"Arena main image deleted successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string id)
        {
            if (!this.ModelState.IsValid)
            {
                this.TempData["message"] = $"Error deleting image occured!";

                return this.RedirectToAction(nameof(this.EditImages));
            }

            await this.imagesService.DeleteAsync(id);
            this.TempData["message"] = $"Image deleted successfully!";

            return this.RedirectToAction(nameof(this.EditImages));
        }

        public async Task<IActionResult> EditImages()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            var viewModel = await this.arenasService.GetImagesByIdAsync(arenaId);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddImages([Bind("NewImages")]ArenaImagesEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.TempData["message"] = $"Error adding images occured!";

                return this.RedirectToAction(nameof(this.EditImages));
            }

            var userId = this.userManager.GetUserId(this.User);
            var arenaId = await this.arenasService.GetIdByAdminIdAsync(userId);
            await this.arenasService.AddImagesAsync(viewModel.NewImages, arenaId);
            this.TempData["message"] = $"New images added successfully!";

            return this.Redirect($"/Arenas/{nameof(this.EditImages)}/{arenaId}");
        }
    }
}
