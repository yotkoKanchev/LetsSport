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
        Task<IEnumerable<EventUserViewModel>> GetAllByEventIdAsync(int id);

        Task UpdateAsync(UserUpdateInputModel inputModel, string userId, string userEmail, string username);

        Task<T> GetDetailsAsync<T>(string id);

        Task<UserUpdateInputModel> GetDetailsForEditAsync(string id, int countryId);

        Task<string> GetUserNameByUserIdAsync(string reportedUserId);

        // imgs
        Task ChangeAvatarAsync(string userId, IFormFile newAvatar);

        string GetUserAvatarUrl(string userId);

        Task DeleteAvatar(string id);

        Task<bool> IsProfileUpdatedAsync(string userId);

        Task<IEnumerable<UserForInvitationModel>> GetAllUsersDetailsForIvitationAsync(string sport, int arenaCityId);

        Task<bool> IsUserHasArenaAsync(string userId);

        // Admin
        Task BlockUserAsync(string reportedUserId);
    }
}
