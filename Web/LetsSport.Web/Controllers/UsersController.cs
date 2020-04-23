namespace LetsSport.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.ConfirmationMessages;
    using static LetsSport.Common.GlobalConstants;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUsersService usersService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILocationLocator locationLocator,
            IUsersService usersService,
            ICountriesService countriesService,
            ISportsService sportsService,
            ICitiesService citiesService)
            : base(locationLocator)
        {
            this.usersService = usersService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);
            var isUserUpdated = await this.usersService.IsProfileUpdatedAsync(userId);

            if (isUserUpdated == true)
            {
                var viewModel = await this.usersService.GetDetailsByIdAsync<UserMyDetailsViewModel>(userId);
                viewModel.AvatarUrl = this.usersService.SetAvatarImage(viewModel.AvatarUrl);

                return this.View(viewModel);
            }

            return this.RedirectToAction(nameof(this.Edit));
        }

        public async Task<IActionResult> Edit()
        {
            this.SetLocation();
            var location = this.GetLocation();
            var user = await this.userManager.GetUserAsync(this.User);
            var countryId = await this.countriesService.GetIdAsync(location.Country);

            if (await this.citiesService.IsExistsAsync(location.City, countryId) == false)
            {
                await this.citiesService.CreateAsync(location.City, countryId);
            }

            var viewModel = await this.usersService.GetDetailsForEditAsync(user.Id, countryId, location.City);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetLocation();
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
            await this.usersService.UpdateAsync(inputModel, user.Id, user.Email, user.UserName);
            this.TempData[TempDataMessage] = UserUpdated;

            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (id == userId)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var viewModel = await this.usersService.GetDetailsByIdAsync<UserDetailsViewModel>(id);
            viewModel.AvatarUrl = this.usersService.SetAvatarImage(viewModel.AvatarUrl);

            return this.View(viewModel);
        }

        [HttpPost]

        public async Task<IActionResult> ChangeAvatar([Bind("NewAvatarImage")]UserMyDetailsViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var id = this.userManager.GetUserId(this.User);
                this.TempData[TempDataMessage] = AddingImageError;
                return this.RedirectToAction(nameof(this.Details), new { id });
            }

            var userId = this.userManager.GetUserId(this.User);

            if (inputModel.NewAvatarImage == null)
            {
                this.TempData[TempDataMessage] = NoPicture;
            }
            else
            {
                await this.usersService.ChangeAvatarAsync(userId, inputModel.NewAvatarImage);
                this.TempData[TempDataMessage] = UserAvatarUpdated;
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar()
        {
            var id = this.userManager.GetUserId(this.User);
            await this.usersService.DeleteAvatar(id);
            this.TempData[TempDataMessage] = UserAvatarDeleted;

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
