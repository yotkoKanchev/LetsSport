namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;

    public interface IUsersService
    {
        Task<string> FillAdditionalUserInfo(UserUpdateInputModel inputModel, string userId);

        UserDetailsViewModel GetDetails(string id);

        UserEditViewModel GetDetailsForEdit(string id);

        Task UpdateAsync(UserEditViewModel inputModel);

        IEnumerable<EventUserViewModel> GetUsersByEventId(int id);

        string GetUserAvatarUrl(string userId);

        IList<Event> GetUserEvents(string userId);
    }
}
