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
        public IActionResult Index()
        {
            this.SetLocation();
            var location = this.GetLocation();
            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = this.arenasService.GetAllInCity<ArenaCardPartialViewModel>(location).ToList(),
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithArenas(location.Country),
                    Sports = this.sportsService.GetAllSportsInCountry(location.Country),
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
        public IActionResult Filter(int sport, int city)
        {
            var location = this.GetLocation();

            var viewModel = this.arenasService.FilterArenas(location.Country, sport, city);

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

            if (user.AdministratingArena != null)
            {
                return this.RedirectToAction(nameof(this.MyArena));
            }

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

            viewModel.MainImageUrl = this.arenasService.SetMainImage(viewModel.MainImageUrl);

            viewModel.Pictures = this.arenasService.GetImageUrslById(id);
            var userId = this.userManager.GetUserId(this.User);

            if (viewModel.ArenaAdminId == userId)
            {
                return this.RedirectToAction(nameof(this.MyArena));
            }

            if (viewModel == null)
            {
                return this.NotFound();
            }

            return this.View(viewModel);
        }

        public IActionResult MyArena()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.arenasService.IsArenaExists(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            var viewModel = this.arenasService.GetDetails<MyArenaDetailsViewModel>(arenaId);
            viewModel.MainImageUrl = this.arenasService.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = this.arenasService.GetImageUrslById(arenaId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.LoggedUserId = this.userManager.GetUserId(this.User);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Events()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.arenasService.IsArenaExists(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var country = this.GetLocation().Country;
            var viewModel = await this.eventsService.GetArenaEventsByArenaAdminId(userId, country);

            return this.View(viewModel);
        }

        public IActionResult Edit()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);

            var inputModel = this.arenasService.GetArenaForEdit(arenaId);

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

            if (!this.ModelState.IsValid)
            {
                var inputModel = this.arenasService.GetArenaForEdit(viewModel.Id);

                return this.View(inputModel);
            }

            await this.arenasService.UpdateArenaAsync(viewModel);
            this.TempData["message"] = $"{viewModel.Name} Arena info updated successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMainImage(MyArenaDetailsViewModel viewModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);

            if (viewModel.NewMainImage == null)
            {
                return this.NoContent();
            }

            await this.arenasService.ChangeMainImageAsync(arenaId, viewModel.NewMainImage);
            this.TempData["message"] = $"{viewModel.Name} Arena main image changed successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMainImage()
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            await this.arenasService.DeleteMainImage(arenaId);
            this.TempData["message"] = $"Arena main image deleted successfully!";

            return this.RedirectToAction(nameof(this.MyArena));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string id)
        {
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
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            var viewModel = this.arenasService.GetArenaImagesByArenaId(arenaId);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddImages(ArenaImagesEditViewModel viewModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            await this.arenasService.AddImages(viewModel.NewImages, arenaId);
            this.TempData["message"] = $"New images added successfully!";

            return this.Redirect($"/Arenas/{nameof(this.EditImages)}/{arenaId}");
        }
    }
}
