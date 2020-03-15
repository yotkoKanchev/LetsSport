namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.UsersProfile;

    public interface IUsersProfileService
    {
        Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId);

        UserProfileDetailsViewModel GetDetails(string id);

        UserProfileEditViewModel GetDetailsForEdit(string id);

        Task UpdateAsync(UserProfileEditViewModel inputModel);

        IEnumerable<EventUserViewModel> GetUsersByEventId(int id);

        string GetUserAvatarUrl(string userId);
    }
}
