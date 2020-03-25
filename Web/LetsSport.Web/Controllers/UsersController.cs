namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ICountriesService countriesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";

        public UsersController(
            IUsersService usersService,
            ICountriesService countriesService,
            IImagesService imagesService,
            ISportsService sportsService,
            ICitiesService citiesService,
            ILocationLocator locationLocator,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
            : base(locationLocator)
        {
            this.usersService = usersService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
            this.userManager = userManager;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<IActionResult> Update()
        {
            var location = this.GetLocation();

            var viewModel = new UserUpdateInputModel
            {
                Sports = this.sportsService.GetAll(),
                Countries = this.countriesService.GetAll(),
                Cities = await this.citiesService.GetCitiesAsync(location),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateInputModel inputModel)
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

            var id = await this.usersService.FillAdditionalUserInfo(inputModel, user.Id, user.Email, user.UserName);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [AllowAnonymous]
        public IActionResult Details(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (id == userId)
            {
                return this.RedirectToAction(nameof(this.MyDetails));
            }

            var viewModel = this.usersService.GetDetails<UserDetailsViewModel>(id);
            viewModel.AvatarUrl = viewModel.AvatarUrl == null
                ? "~/images/noAvatar.png"
                : this.imagePathPrefix + this.avatarImageSizing + viewModel.AvatarUrl;

            return this.View(viewModel);
        }

        public IActionResult MyDetails()
        {
            var userId = this.userManager.GetUserId(this.User);

            var isUserUpdated = this.usersService.IsUserProfileUpdated(userId);

            if (isUserUpdated == true)
            {
                var viewModel = this.usersService.GetDetails<UserMyDetailsViewModel>(userId);
                viewModel.AvatarUrl = viewModel.AvatarUrl == null
                ? "~/images/noAvatar.png"
                : this.imagePathPrefix + this.avatarImageSizing + viewModel.AvatarUrl;
                return this.View(viewModel);
            }

            return this.RedirectToAction(nameof(this.Update));
        }

        public IActionResult Edit(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (userId != id)
            {
                return new ForbidResult();
            }

            var isUserUpdated = this.usersService.IsUserProfileUpdated(id);

            if (isUserUpdated == true)
            {
                var viewModel = this.usersService.GetDetailsForEdit(id);
                return this.View(viewModel);
            }

            return this.RedirectToAction(nameof(this.Update), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel inputModel)
        {
            var id = this.userManager.GetUserId(this.User);

            if (id != inputModel.Id)
            {
                return new ForbidResult();
            }

            await this.usersService.UpdateAsync(inputModel);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(UserMyDetailsViewModel viewModel)
        {
            var id = this.userManager.GetUserId(this.User);

            if (id != viewModel.Id)
            {
                return new ForbidResult();
            }

            if (viewModel.NewAvatarImage == null)
            {
                return this.NoContent();
            }

            await this.usersService.ChangeAvatarAsync(id, viewModel.NewAvatarImage);
            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar()
        {
            // TODO add validation for the user
            var id = this.userManager.GetUserId(this.User);
            await this.usersService.DeleteAvatar(id);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }
    }
}
