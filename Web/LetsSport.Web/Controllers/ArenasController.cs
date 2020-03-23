namespace LetsSport.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

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
                Arenas = this.arenasService.GetAll<ArenaIndexInfoViewModel>(location).ToList(),
                Cities = this.citiesService.GetCitiesWithArenasAsync(location.Country),
            };

            foreach (var model in viewModel.Arenas)
            {
                model.MainImageUrl = string.IsNullOrEmpty(model.MainImageUrl) ? "../../images/noArena.png" : this.imagePathPrefix + model.MainImageUrl;
            }

            return this.View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ChangeCity(ArenaIndexListViewModel inputModel)
        {
            var location = this.GetLocation();

            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = this.arenasService.GetArenasByCityId(inputModel.City),
                Cities = this.citiesService.GetCitiesWithArenasAsync(location.Country),
            };

            foreach (var model in viewModel.Arenas)
            {
                model.MainImageUrl = string.IsNullOrEmpty(model.MainImageUrl) ? "../../images/noArena.png" : this.imagePathPrefix + model.MainImageUrl;
            }

            this.ViewData["location"] = this.citiesService.GetLocationByCityId(inputModel.City);

            return this.View(nameof(this.Index), viewModel);
        }

        [Authorize(Roles = "ArenaAdministrator")]
        public async Task<IActionResult> Create()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.arenasService.IsArenaExists(userId) == true)
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
        [Authorize(Roles = "ArenaAdministrator")]
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
            var id = await this.arenasService.CreateAsync(inputModel, user.Id, user.Email, user.UserName);

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

            return this.View(viewModel);
        }

        [Authorize(Roles = "ArenaAdministrator")]
        public IActionResult MyArena()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.arenasService.IsArenaExists(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            var viewModel = this.arenasService.GetMyArenaDetails(arenaId);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            viewModel.LoggedUserId = this.userManager.GetUserId(this.User);

            return this.View(viewModel);
        }

        [Authorize(Roles = "ArenaAdministrator")]
        public IActionResult Events()
        {
            var userId = this.userManager.GetUserId(this.User);

            if (this.arenasService.IsArenaExists(userId) == false)
            {
                return this.RedirectToAction(nameof(this.Create));
            }

            var viewModel = this.eventsService.GetArenaEventsByArenaAdminId(userId);

            return this.View(viewModel);
        }

        [Authorize(Roles = "ArenaAdministrator")]
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
        [Authorize(Roles = "ArenaAdministrator")]
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
        [Authorize(Roles = "ArenaAdministrator")]
        public async Task<IActionResult> ChangeMainImage(MyArenaDetailsViewModel viewModel, int id)
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
        [Authorize(Roles = "ArenaAdministrator")]
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
        [Authorize(Roles = "ArenaAdministrator")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var arenaId = this.arenasService.GetArenaIdByAdminId(userId);
            await this.imagesService.DeleteImageAsync(id);

            return this.Redirect($"/Arenas/{nameof(this.EditImages)}/{arenaId}");
        }

        [HttpPost]
        [Authorize(Roles = "ArenaAdministrator")]
        public IActionResult DeleteImages()
        {
            // TODO has to be done with JavaScript
            return this.Ok();
        }

        [Authorize(Roles = "ArenaAdministrator")]
        public IActionResult EditImages(int id)
        {
            var viewModel = this.arenasService.GetArenasImagesByArenaId(id);

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "ArenaAdministrator")]
        public async Task<IActionResult> AddImages(ArenaImagesEditViewModel viewModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.arenasService.AddImages(viewModel.NewImages, viewModel.Id);

            return this.Redirect($"/Arenas/{nameof(this.EditImages)}/{viewModel.Id}");
        }
    }
}
