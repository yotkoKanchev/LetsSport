namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ICountriesService countriesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_ppq2gm.png";

        public UsersController(
            IUsersService usersService,
            ICountriesService countriesService,
            IImagesService imagesService,
            ISportsService sportsService,
            ICitiesService citiesService,
            ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.usersService = usersService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
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

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = await this.usersService.FillAdditionalUserInfo(inputModel, userId);

            return this.Redirect($"details/{id}");
        }

        [AllowAnonymous]
        public IActionResult Details(string id)
        {
            var viewModel = this.usersService.GetDetails(id);

            return this.View(viewModel);
        }

        public IActionResult Edit(string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != inputModel.Id)
            {
                return new ForbidResult();
            }

            await this.usersService.UpdateAsync(inputModel);

            return this.Redirect($"/users/details/{inputModel.Id}");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(UserDetailsViewModel viewModel, string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != viewModel.Id)
            {
                return new ForbidResult();
            }

            if (viewModel.NewAvatarImage != null)
            {
                await this.imagesService.ChangeImageAsync(viewModel.NewAvatarImage, id);

                return this.Redirect($"/users/details/{userId}");
            }

            return this.Redirect($"/users/details/{userId}");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar(UserDetailsViewModel viewModel, string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != viewModel.Id)
            {
                return new ForbidResult();
            }

            await this.imagesService.DeleteImageAsync(id, this.noAvatarUrl);

            return this.Redirect($"/users/details/{userId}");
        }
    }
}
