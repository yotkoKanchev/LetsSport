namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class UsersService : IUsersService
    {
        private const string InvalidUserIdErrorMessage = "User with ID: {0} does not exist.";

        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly IEmailSender emailSender;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;
        private readonly IConfiguration configuration;
        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<EventUser> eventsUsersRepository,
            IEmailSender emailSender,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IImagesService imagesService,
            IConfiguration configuration)
        {
            this.eventsUsersRepository = eventsUsersRepository;
            this.emailSender = emailSender;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
            this.usersRepository = usersRepository;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<IEnumerable<EventUserViewModel>> GetAllByEventIdAsync(int id)
        {
            return await this.eventsUsersRepository.All()
                .Where(ev => ev.EventId == id)
                .OrderBy(ev => ev.User.UserName)
                .To<EventUserViewModel>()
                .ToListAsync();
        }

        public async Task FillAdditionalUserInfoAsync(UserUpdateInputModel inputModel, string userId, string userEmail, string username)
        {
            var user = await this.GetUserById(userId).FirstAsync();

            user.FirstName = inputModel.FirstName;
            user.LastName = inputModel.LastName;
            user.UserName = inputModel.UserName;
            user.Gender = inputModel.Gender;
            user.SportId = inputModel.SportId;
            user.Status = inputModel.Status;
            user.CountryId = inputModel.CountryId;
            user.CityId = inputModel.CityId;
            user.PhoneNumber = inputModel.PhoneNumber;
            user.FaceBookAccount = inputModel.FaceBookAccount;
            user.Age = inputModel.Age;
            user.Occupation = inputModel.Occupation;
            user.IsUserProfileUpdated = true;

            if (inputModel.AvatarImage != null)
            {
                var avatar = await this.imagesService.CreateAsync(inputModel.AvatarImage);
                user.AvatarId = avatar.Id;
            }

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(username));
        }

        public async Task<T> GetDetailsAsync<T>(string id)
        {
            var query = this.GetUserById(id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, id));
            }

            var viewModel = await query.To<T>().FirstOrDefaultAsync();

            return viewModel;
        }

        public async Task<UserEditViewModel> GetDetailsForEditAsync(string id)
        {
            var query = this.GetUserById(id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, id));
            }

            var viewModel = await query.To<UserEditViewModel>().FirstOrDefaultAsync();

            viewModel.Countries = this.countriesService.GetAllAsSelectList();
            viewModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(viewModel.CountryId);
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();

            return viewModel;
        }

        public async Task UpdateAsync(UserEditViewModel inputModel)
        {
            var userProfile = await this.GetUserById(inputModel.Id).FirstAsync();

            if (userProfile == null)
            {
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, inputModel.Id));
            }

            userProfile.FirstName = inputModel.FirstName;
            userProfile.LastName = inputModel.LastName;
            userProfile.UserName = inputModel.UserName;
            userProfile.Gender = inputModel.Gender;
            userProfile.SportId = inputModel.SportId;
            userProfile.Status = inputModel.Status;
            userProfile.CountryId = inputModel.CountryId;
            userProfile.CityId = inputModel.CityId;
            userProfile.PhoneNumber = inputModel.PhoneNumber;
            userProfile.FaceBookAccount = inputModel.FaceBookAccount;
            userProfile.Age = inputModel.Age;
            userProfile.Occupation = inputModel.Occupation;

            this.usersRepository.Update(userProfile);
            await this.usersRepository.SaveChangesAsync();
        }

        // TODO move getting user avatar url from loginv view to login page and make this method async
        public string GetUserAvatarUrl(string userId)
        {
            var avatarUrl = this.GetUserById(userId)
                .Select(up => up.Avatar.Url)
                .FirstOrDefault();

            return avatarUrl == null ? "~/images/noAvatar.png" : this.imagePathPrefix + this.avatarImageSizing + avatarUrl;
        }

        public async Task ChangeAvatarAsync(string userId, IFormFile newAvatarFile)
        {
            var user = await this.GetUserById(userId).FirstAsync();

            var oldAvatarId = user.AvatarId;
            var newAvatar = await this.imagesService.CreateAsync(newAvatarFile);
            user.AvatarId = newAvatar.Id;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            await this.imagesService.DeleteAsync(oldAvatarId);

            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
        }

        public async Task<bool> IsUserProfileUpdatedAsync(string userId)
        {
            var user = await this.GetUserById(userId).FirstAsync();
            return user.IsUserProfileUpdated;
        }

        public async Task DeleteAvatar(string userId)
        {
            var user = await this.GetUserById(userId).FirstAsync();
            var avatarId = user.AvatarId;
            user.AvatarId = null;

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
            await this.imagesService.DeleteAsync(avatarId);
            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
        }

        public async Task<IEnumerable<UserForInvitationModel>> GetAllUsersDetailsForIvitationAsync(string sport, int arenaCityId)
        {
            var users = await this.usersRepository
                .All()
                .Where(u => u.CityId == arenaCityId)
                .Where(u => u.Sport.Name == sport)
                .Select(u => new UserForInvitationModel
                {
                    Email = u.Email,
                    Username = u.UserName,
                })
                .ToListAsync();

            return users;
        }

        public async Task<string> GetUserNameByUserIdAsync(string reportedUserId)
        {
            return await this.GetUserById(reportedUserId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }

        public async Task BlockUserAsync(string userId)
        {
            var user = this.GetUserById(userId).First();
            user.IsDeleted = true;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task<bool> IsUserHasArenaAsync(string userId)
        {
            var result = await this.GetUserById(userId)
                .Select(u => u.AdministratingArena)
                .FirstOrDefaultAsync();

            return result == null ? false : true;
        }

        private IQueryable<ApplicationUser> GetUserById(string userId)
        {
            var user = this.usersRepository
                .All()
                .Where(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException($"User with ID: {userId} does not exists!");
            }

            return user;
        }
    }
}
