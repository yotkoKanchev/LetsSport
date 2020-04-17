namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.Cloudinary;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;
    using static LetsSport.Common.GlobalConstants;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly IEmailSender emailSender;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;
        private readonly string imagePathPrefix;

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<EventUser> eventsUsersRepository,
            IEmailSender emailSender,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            ICloudinaryHelper cloudinaryHelper,
            IImagesService imagesService)
        {
            this.eventsUsersRepository = eventsUsersRepository;
            this.emailSender = emailSender;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
            this.usersRepository = usersRepository;
            this.imagePathPrefix = cloudinaryHelper.GetPrefix();
        }

        public async Task<IEnumerable<EventUserViewModel>> GetAllByEventIdAsync(int id)
        {
            return await this.eventsUsersRepository.All()
                .Where(ev => ev.EventId == id)
                .OrderBy(ev => ev.User.UserName)
                .To<EventUserViewModel>()
                .ToListAsync();
        }

        public async Task UpdateAsync(UserUpdateInputModel inputModel, string userId, string userEmail, string username)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();

            user.FirstName = inputModel.FirstName;
            user.LastName = inputModel.LastName;
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
            var query = this.GetUserByIdAsIQueryable(id);
            var viewModel = query.To<T>();

            return await viewModel.FirstOrDefaultAsync();
        }

        public async Task<UserUpdateInputModel> GetDetailsForEditAsync(string id, int countryId, string cityName)
        {
            var query = this.GetUserByIdAsIQueryable(id);
            var viewModel = await query.To<UserUpdateInputModel>().FirstOrDefaultAsync();
            viewModel.Countries = await this.countriesService.GetAllAsSelectListAsync();
            viewModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId);
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();
            viewModel.CountryId = countryId;
            viewModel.CityId = await this.citiesService.GetIdAsync(cityName, countryId);

            return viewModel;
        }

        public string GetUserAvatarUrl(string userId)
        {
            var avatarUrl = this.GetUserByIdAsIQueryable(userId)
                .Select(up => up.Avatar.Url)
                .FirstOrDefault();

            return avatarUrl == null
                ? NoAvatarImagePath
                : this.imagePathPrefix + AvatarImageSizing + avatarUrl;
        }

        public async Task ChangeAvatarAsync(string userId, IFormFile newAvatarFile)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();
            var oldAvatarId = user.AvatarId;
            var newAvatar = await this.imagesService.CreateAsync(newAvatarFile);
            user.AvatarId = newAvatar.Id;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            if (oldAvatarId != null)
            {
                await this.imagesService.DeleteAsync(oldAvatarId);
            }

            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
        }

        public async Task<bool> IsProfileUpdatedAsync(string userId)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();
            return user.IsUserProfileUpdated;
        }

        public async Task DeleteAvatar(string userId)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();
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

        public async Task<IEnumerable<EmailUserInfo>> GetAllUsersDetailsForIvitationAsync(string sport, int arenaCityId)
        {
            var users = await this.usersRepository.All()
                .Where(u => u.CityId == arenaCityId)
                .Where(u => u.Sport.Name == sport)
                .Select(u => new EmailUserInfo
                {
                    Email = u.Email,
                    Username = u.UserName,
                })
                .ToListAsync();

            return users;
        }

        public string SetAvatarImage(string imageUrl)
        {
            var resultUrl = NoAvatarImagePath;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imagePath = this.imagesService.ConstructUrlPrefix(AvatarImageSizing);
                resultUrl = imagePath + imageUrl;
            }

            return resultUrl;
        }

        public async Task<string> GetUserNameByUserIdAsync(string reportedUserId)
        {
            return await this.GetUserByIdAsIQueryable(reportedUserId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }

        public async Task BlockUserAsync(string userId)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();
            user.IsDeleted = true;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task<bool> IsUserHasArenaAsync(string userId)
        {
            var result = await this.GetUserByIdAsIQueryable(userId)
                .Select(u => u.AdministratingArena)
                .FirstOrDefaultAsync();

            return result == null ? false : true;
        }

        private IQueryable<ApplicationUser> GetUserByIdAsIQueryable(string userId)
        {
            var user = this.usersRepository.All()
                .Where(u => u.Id == userId);

            if (!user.Any())
            {
                throw new ArgumentException(string.Format(UserInvalidIdErrorMessage, userId));
            }

            return user;
        }
    }
}
