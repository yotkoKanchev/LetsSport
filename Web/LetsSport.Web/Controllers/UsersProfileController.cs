namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.UsersProfile;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class UsersProfileController : BaseController
    {
        private readonly IUsersProfileService usersProfileService;
        private readonly ICountriesService countriesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly ICitiesService citiesService;

        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_qjeerp.png";

        public UsersProfileController(
            IUsersProfileService usersProfileService,
            ICountriesService countriesService,
            IImagesService imagesService,
            ISportsService sportsService,
            ICitiesService citiesService)
        {
            this.usersProfileService = usersProfileService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.citiesService = citiesService;
        }

        public async Task<IActionResult> Create()
        {
            var city = this.HttpContext.Session.GetString("city");
            var country = this.HttpContext.Session.GetString("country");

            var viewModel = new UserProfileCreateInputModel
            {
                Sports = this.sportsService.GetAll(),
                Countries = this.countriesService.GetAll(),
                Cities = await this.citiesService.GetCitiesAsync(city, country),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserProfileCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var city = this.HttpContext.Session.GetString("city");
                var country = this.HttpContext.Session.GetString("country");
                inputModel.Sports = this.sportsService.GetAll();
                inputModel.Countries = this.countriesService.GetAll();
                inputModel.Cities = await this.citiesService.GetCitiesAsync(city, country);

                return this.View(inputModel);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = await this.usersProfileService.CreateUserProfile(inputModel, userId);

            return this.Redirect($"details/{id}");
        }

        public IActionResult Details(string id)
        {
            var viewModel = this.usersProfileService.GetDetails(id);

            return this.View(viewModel);
        }

        public IActionResult Edit(string id)
        {
            var viewModel = this.usersProfileService.GetDetailsForEdit(id);
            //var countries = this.countriesService.GetAll();
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileEditViewModel inputModel)
        {
            await this.usersProfileService.UpdateAsync(inputModel);

            return this.Redirect($"/usersprofile/details/{inputModel.Id}");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(UserProfileDetailsViewModel viewModel, string id)
        {
            // TODO throw Exception file not selected
            if (viewModel.NewAvatarImage != null)
            {
                await this.imagesService.ChangeImageAsync(viewModel.NewAvatarImage, id);
                return this.Redirect($"/usersprofile/details/{viewModel.UserProfileId}");
            }

            return this.Redirect($"/usersprofile/details/{viewModel.UserProfileId}");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvatar(UserProfileDetailsViewModel viewModel, string id)
        {
            await this.imagesService.DeleteImageAsync(id, this.noAvatarUrl);
            return this.Redirect($"/usersprofile/details/{viewModel.UserProfileId}");
        }
    }
}
