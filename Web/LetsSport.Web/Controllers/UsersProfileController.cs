namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;

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
        public IActionResult Create(UserProfileCreateInputModel inputModel)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            this.usersProfileService.CreateUserProfile(inputModel, userId);

            return null;
        }
    }
}
