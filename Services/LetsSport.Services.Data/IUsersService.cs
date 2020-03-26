namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;

    public interface IUsersService
    {
        Task ChangeAvatarAsync(string userId, IFormFile newAvatar);

        Task<string> FillAdditionalUserInfo(UserUpdateInputModel inputModel, string userId, string userEmail, string username);

        T GetDetails<T>(string id);

        UserEditViewModel GetDetailsForEdit(string id);

        Task UpdateAsync(UserEditViewModel inputModel);

        IEnumerable<EventUserViewModel> GetUsersByEventId(int id);

        string GetUserAvatarUrl(string userId);

        IList<Event> GetUserEvents(string userId);

        bool IsUserProfileUpdated(string userId);

        Task DeleteAvatar(string id);

        string GetArenaAdminIdByArenaId(int arenaId);

        public IEnumerable<UserForInvitationModel> GetAllUsersDetailsForIvitation(string sport, int arenaCityId);
    }
}
