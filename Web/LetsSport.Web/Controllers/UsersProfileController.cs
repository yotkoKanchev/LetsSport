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

        public UsersProfileController(IUsersProfileService usersProfileService, ICountriesService countriesService, IImagesService imagesService)
        {
            this.usersProfileService = usersProfileService;
            this.countriesService = countriesService;
            this.imagesService = imagesService;
        }

        public IActionResult Create()
        {
            var countries = this.countriesService.GetAll();
            this.ViewData["countries"] = countries;
            this.ViewData["city"] = this.HttpContext.Session.GetString("city");
            this.ViewData["country"] = this.HttpContext.Session.GetString("country");
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserProfileCreateInputModel inputModel)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.usersProfileService.CreateUserProfile(inputModel, userId);

            return this.Redirect($"details/{userId}");
        }

        public IActionResult Details(string id)
        {
            var viewModel = this.usersProfileService.GetDetails(id);

            return this.View(viewModel);
        }

        public IActionResult Edit(string id)
        {
            var viewModel = this.usersProfileService.GetDetailsForEdit(id);
            var countries = this.countriesService.GetAll();
            this.ViewData["countries"] = countries;
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileEditInputModel inputModel)
        {
            await this.usersProfileService.UpdateAsync(inputModel);

            return this.Redirect($"/usersprofile/details/{inputModel.Id}");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePicture(UserProfileDetailsViewModel viewModel, string id)
        {
            await this.imagesService.ChangeImageAsync(viewModel.NewAvatarImage, id);
            return this.Redirect($"/usersprofile/details/{viewModel.UserProfileId}");
        }
    }
}
