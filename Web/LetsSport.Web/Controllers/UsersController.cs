namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ICountriesService countriesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            IUsersService usersService,
            ICountriesService countriesService,
            IImagesService imagesService,
            ISportsService sportsService,
            ICitiesService citiesService,
            ILocationLocator locationLocator,
            UserManager<ApplicationUser> userManager)
            : base(locationLocator)
        {
            this.usersService = usersService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Update()
        {
            this.SetLocation();
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

            var userId = this.userManager.GetUserId(this.User);
            var id = await this.usersService.FillAdditionalUserInfo(inputModel, userId);

            return this.RedirectToAction(nameof(this.Details), new { id });
        }

        [AllowAnonymous]
        public IActionResult Details(string id)
        {
            var isUserUpdated = this.usersService.IsUserProfileUpdated(id);

            if (isUserUpdated == true)
            {
                var viewModel = this.usersService.GetDetails(id);
                return this.View(viewModel);
            }

            var userId = this.userManager.GetUserId(this.User);

            if (userId != id)
            {
                return this.Unauthorized();
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

            var viewModel = this.usersService.GetDetailsForEdit(id);
            return this.View(viewModel);
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
        public async Task<IActionResult> ChangeAvatar(UserDetailsViewModel viewModel)
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

            var newAvatarId = await this.imagesService.ChangeImageAsync(viewModel.NewAvatarImage, id);
            await this.usersService.ChangeAvatarAsync(id, newAvatarId);
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
