﻿namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.UsersProfile;

    public interface IUsersProfileService
    {
        public Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId);

        public UserProfileDetailsViewModel GetDetails(string id);

        public UserProfileEditViewModel GetDetailsForEdit(string id);

        public Task UpdateAsync(UserProfileEditViewModel inputModel);

        IEnumerable<EventUserViewModel> GetUsersByEventId(int id);
    }
}
