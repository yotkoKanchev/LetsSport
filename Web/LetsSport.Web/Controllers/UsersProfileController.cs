namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.UsersProfile;
    using Microsoft.AspNetCore.Mvc;

    public class UsersProfileController : BaseController
    {
        private readonly IUsersProfileService usersProfileService;

        public UsersProfileController(IUsersProfileService usersProfileService)
        {
            this.usersProfileService = usersProfileService;
        }

        public IActionResult Create()
        {
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

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileEditInputModel inputModel)
        {
            await this.usersProfileService.UpdateAsync(inputModel);

            return this.Redirect($"/usersprofile/details/{inputModel.Id}");
        }
    }
}
