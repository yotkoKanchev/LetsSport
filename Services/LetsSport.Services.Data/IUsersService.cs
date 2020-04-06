namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;

    public interface IUsersService
    {
        IEnumerable<EventUserViewModel> GetAllByEventId(int id);

        Task FillAdditionalUserInfo(UserUpdateInputModel inputModel, string userId, string userEmail, string username);

        T GetDetails<T>(string id);

        Task<UserEditViewModel> GetDetailsForEditAsync(string id);

        Task UpdateAsync(UserEditViewModel inputModel);

        string GetUserNameByUserId(string reportedUserId);

        // imgs
        Task ChangeAvatarAsync(string userId, IFormFile newAvatar);

        string GetUserAvatarUrl(string userId);

        Task DeleteAvatar(string id);

        bool IsUserProfileUpdated(string userId);

        public IEnumerable<UserForInvitationModel> GetAllUsersDetailsForIvitation(string sport, int arenaCityId);

        bool IsUserHasArena(string userId);

        // Admin
        Task BlockUserAsync(string reportedUserId);
    }
}
