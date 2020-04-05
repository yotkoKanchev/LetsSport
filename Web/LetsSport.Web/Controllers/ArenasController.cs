namespace LetsSport.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Authorize]
    [Authorize(Roles = GlobalConstants.ArenaAdminRoleName)]
    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IUsersService usersService;
        private readonly IImagesService imagesService;
        private readonly IEventsService eventsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string imagePathPrefix;
        private readonly string noArenaImageUrl = "../../images/noArena.png";

        public ArenasController(
            IConfiguration configuration,
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
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            this.SetLocation();
            var location = this.GetLocation();
            var countryId = this.countriesService.GetId(location.Country);
            var cityId = await this.citiesService.GetIdAsync(location.City, countryId);

            await this.eventsService.SetPassedStatusOnPassedEvents(countryId);

            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = await this.arenasService.GetAllInCityAsync<ArenaCardPartialViewModel>(cityId),
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithArenasInCountryAsync(countryId),
                    Sports = await this.sportsService.GetAllSportsInCountryByIdAsync(countryId),
                },
            };

            foreach (var model in viewModel.Arenas)
            {
                model.MainImageUrl = model.MainImageUrl != null
                    ? this.imagePathPrefix + model.MainImageUrl
                    : this.noArenaImageUrl;
            }

            return this.View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(int sport, int city)
        {
            var location = this.GetLocation();
            var countryId = this.countriesService.GetId(location.Country);
            var viewModel = await this.arenasService.FilterAsync(countryId, sport, city);

            foreach (var model in viewModel.Arenas)
            {
                model.MainImageUrl = model.MainImageUrl != null
                    ? this.imagePathPrefix + model.MainImageUrl
                    : this.noArenaImageUrl;
            }

            this.ViewData["location"] = city == 0
                ? location.Country
                : this.citiesService.GetLocationByCityId(city);

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (this.usersService.IsUserHasArena(user.Id))
            {
                return this.RedirectToAction(nameof(this.MyArena));
            }

            var location = this.GetLocation();
            var countryId = this.countriesService.GetId(location.Country);

            var viewModel = new ArenaCreateInputModel
            {
                Sports = this.sportsService.GetAllAsSelectList(),
                Countries = this.countriesService.GetAllAsSelectList(),
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
                var countryId = this.countriesService.GetId(location.Country);
                inputModel.Sports = this.sportsService.GetAllAsSelectList();
                inputModel.Countries = this.countriesService.GetAllAsSelectList();
                inputModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId);
                inputModel.CountryId = countryId;
                inputModel.CityId = await this.citiesService.GetIdAsync(location.City, countryId);

                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            await this.arenasService.CreateAsync(inputModel, user.Id, user.Email, user.UserName);
            this.TempData["message"] = $"{inputModel.Name} Arena created successfully!";

            // TODO pass filtered by sport Arenas with AJAX;
            return this.RedirectToAction(nameof(this.MyArena));
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var viewModel = this.arenasService.GetDetails<ArenaDetailsViewModel>(id);

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
            viewModel.Pictures = this.arenasService.GetImagesUrslById(id);

            return this.View(viewModel);
        }

        public IActionResult MyArena()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.usersService.IsUserHasArena(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var arenaId = this.arenasService.GetIdByAdminId(userId);
            var viewModel = this.arenasService.GetDetails<MyArenaDetailsViewModel>(arenaId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.MainImageUrl = this.arenasService.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = this.arenasService.GetImagesUrslById(arenaId);
            viewModel.LoggedUserId = userId;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Events()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.usersService.IsUserHasArena(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var countryName = this.GetLocation().Country;
            var countryId = this.countriesService.GetId(countryName);
            await this.eventsService.SetPassedStatusOnPassedEvents(countryId);
            var viewModel = await this.eventsService.GetArenaEventsByArenaAdminId(userId);

            return this.View(viewModel);
        }

        public IActionResult Edit()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetIdByAdminId(userId);

            var inputModel = this.arenasService.GetDetailsForEdit(arenaId);

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
                var inputModel = this.arenasService.GetDetailsForEdit(viewModel.Id);

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
            var arenaId = this.arenasService.GetIdByAdminId(userId);
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
            var arenaId = this.arenasService.GetIdByAdminId(userId);
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

            await this.imagesService.DeleteImageAsync(id);
            this.TempData["message"] = $"Image deleted successfully!";

            return this.RedirectToAction(nameof(this.EditImages));
        }

        [HttpPost]
        public IActionResult DeleteImages()
        {
            // TODO has to be done with JavaScript
            return this.Ok();
        }

        public IActionResult EditImages()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetIdByAdminId(userId);
            var viewModel = this.arenasService.GetImagesById(arenaId);

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
            var arenaId = this.arenasService.GetIdByAdminId(userId);
            await this.arenasService.AddImagesAsync(viewModel.NewImages, arenaId);
            this.TempData["message"] = $"New images added successfully!";

            return this.Redirect($"/Arenas/{nameof(this.EditImages)}/{arenaId}");
        }
    }
}
