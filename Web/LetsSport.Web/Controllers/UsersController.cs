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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IUsersService usersService;
        private readonly ICountriesService countriesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILocationLocator locationLocator,
            IUsersService usersService,
            ICountriesService countriesService,
            IImagesService imagesService,
            ISportsService sportsService,
            ICitiesService citiesService)
            : base(locationLocator)
        {
            this.usersService = usersService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);

            var isUserUpdated = await this.usersService.IsUserProfileUpdatedAsync(userId);

            if (isUserUpdated == true)
            {
                var viewModel = await this.usersService.GetDetailsAsync<UserMyDetailsViewModel>(userId);
                viewModel.AvatarUrl = viewModel.AvatarUrl == null
                ? "~/images/noAvatar.png"
                : this.imagePathPrefix + this.avatarImageSizing + viewModel.AvatarUrl;
                return this.View(viewModel);
            }

            return this.RedirectToAction(nameof(this.Update));
        }

        public async Task<IActionResult> Update()
        {
            var location = this.GetLocation();
            var user = await this.userManager.GetUserAsync(this.User);
            var countryId = this.countriesService.GetId(location.Country);

            if (this.citiesService.IsExists(location.City, countryId) == false)
            {
                await this.citiesService.CreateAsync(location.City, countryId);
            }

            var viewModel = new UserUpdateInputModel
            {
                Sports = await this.sportsService.GetAllAsSelectListAsync(),
                Countries = this.countriesService.GetAllAsSelectList(),
                Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                UserName = user.UserName,
                CountryName = location.Country,
                CityName = location.City,
                CountryId = countryId,
                CityId = await this.citiesService.GetIdAsync(location.City, countryId),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var location = this.GetLocation();
                var countryId = this.countriesService.GetId(location.Country);

                inputModel.Sports = await this.sportsService.GetAllAsSelectListAsync();
                inputModel.Countries = this.countriesService.GetAllAsSelectList();
                inputModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId);
                inputModel.CountryId = countryId;
                inputModel.CityId = await this.citiesService.GetIdAsync(location.City, countryId);

                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            await this.usersService.FillAdditionalUserInfoAsync(inputModel, user.Id, user.Email, user.UserName);
            this.TempData["message"] = $"Your profile has been updated successfully!";

            return this.RedirectToAction(nameof(this.Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (id == userId)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var viewModel = await this.usersService.GetDetailsAsync<UserDetailsViewModel>(id);
            viewModel.AvatarUrl = viewModel.AvatarUrl == null
                ? "~/images/noAvatar.png"
                : this.imagePathPrefix + this.avatarImageSizing + viewModel.AvatarUrl;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = this.userManager.GetUserId(this.User);
            var isUserUpdated = this.usersService.IsUserProfileUpdatedAsync(userId);

            if (await isUserUpdated == true)
            {
                var viewModel = await this.usersService.GetDetailsForEditAsync(userId);
                return this.View(viewModel);
            }

            return this.RedirectToAction(nameof(this.Update));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel inputModel)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.usersService.UpdateAsync(inputModel);
            this.TempData["message"] = $"Your profile has been updated successfully!";

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(UserMyDetailsViewModel inputModel)
        {
            var id = this.userManager.GetUserId(this.User);

            if (inputModel.NewAvatarImage == null)
            {
                return this.NoContent();
            }

            await this.usersService.ChangeAvatarAsync(id, inputModel.NewAvatarImage);
            this.TempData["message"] = $"Your avatar image has been updated successfully!";

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar()
        {
            var id = this.userManager.GetUserId(this.User);
            await this.usersService.DeleteAvatar(id);
            this.TempData["message"] = $"Your avatar image has been deleted successfully!";

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
