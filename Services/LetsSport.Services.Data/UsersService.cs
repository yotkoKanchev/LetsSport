namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;
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

        public async Task<string> FillAdditionalUserInfo(UserUpdateInputModel inputModel, string userId, string userEmail, string username)
        {
            var profile = this.usersRepository
                .All()
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            profile.FirstName = inputModel.FirstName;
            profile.LastName = inputModel.LastName;
            profile.Age = inputModel.Age;
            profile.Gender = inputModel.Gender;
            profile.Status = inputModel.Status;
            profile.FaceBookAccount = inputModel.FaceBookAccount;
            profile.PhoneNumber = inputModel.PhoneNumber;
            profile.Occupation = inputModel.Occupation;
            profile.CityId = inputModel.CityId;
            profile.SportId = inputModel.SportId;
            profile.IsUserProfileUpdated = true;

            if (inputModel.AvatarImage != null)
            {
                var avatar = await this.imagesService.CreateAsync(inputModel.AvatarImage);
                profile.AvatarId = avatar.Id;
            }

            this.usersRepository.Update(profile);
            await this.usersRepository.SaveChangesAsync();

            var sportName = this.sportsService.GetSportNameById(inputModel.SportId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(username));

            return profile.Id;
        }

        public IList<Event> GetUserEvents(string userId)
        {
            var events = this.usersRepository
                .All()
                .Where(u => u.Id == userId)
                .Select(u => u.Events
                    .Select(ue => ue.Event)
                    .ToList())
                .FirstOrDefault();

            return events;
        }

        public T GetDetails<T>(string id)
        {
            var query = this.usersRepository
                .All()
                .Where(up => up.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, id));
            }

            var viewModel = query.To<T>().FirstOrDefault();

            return viewModel;
        }

        public UserEditViewModel GetDetailsForEdit(string id)
        {
            var query = this.usersRepository
                .All()
                .Where(up => up.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, id));
            }

            var viewModel = query.To<UserEditViewModel>().FirstOrDefault();

            viewModel.Countries = this.countriesService.GetAll();
            viewModel.Cities = this.citiesService.GetCitiesSelectList(viewModel.CityCountryId);
            viewModel.Sports = this.sportsService.GetAll();

            return viewModel;
        }

        public async Task UpdateAsync(UserEditViewModel inputModel)
        {
            var userProfile = this.usersRepository
                .All()
                .Where(up => up.Id == inputModel.Id)
                .FirstOrDefault();

            if (userProfile == null)
            {// TODO refactor exception
                throw new ArgumentNullException(string.Format(InvalidUserIdErrorMessage, inputModel.Id));
            }

            userProfile.FirstName = inputModel.FirstName;
            userProfile.LastName = inputModel.LastName;
            userProfile.Age = inputModel.Age;
            userProfile.PhoneNumber = inputModel.PhoneNumber;
            userProfile.FaceBookAccount = inputModel.FaceBookAccount;
            userProfile.Occupation = inputModel.Occupation;
            userProfile.Gender = inputModel.Gender;
            userProfile.SportId = inputModel.SportId;
            userProfile.Status = inputModel.Status;
            userProfile.CityId = inputModel.CityId;

            this.usersRepository.Update(userProfile);
            await this.usersRepository.SaveChangesAsync();
        }

        public IEnumerable<EventUserViewModel> GetUsersByEventId(int id)
        {
            var query = this.eventsUsersRepository.All()
                .Where(ev => ev.EventId == id)
                .OrderBy(ev => ev.User.UserName);

            var users = query.To<EventUserViewModel>();

            return users.ToList();
        }

        public string GetUserAvatarUrl(string userId)
        {
            var avatarUrl = this.usersRepository.
                All()
                .Where(up => up.Id == userId)
                .Select(up => up.Avatar.Url)
                .FirstOrDefault();

            return avatarUrl == null ? "~/images/noAvatar.png" : this.imagePathPrefix + this.avatarImageSizing + avatarUrl;
        }

        public async Task ChangeAvatarAsync(string userId, IFormFile newAvatarFile)
        {
            var user = this.usersRepository.All()
                .FirstOrDefault(u => u.Id == userId);

            var oldAvatarId = user.AvatarId;
            var newAvatar = await this.imagesService.CreateAsync(newAvatarFile);
            user.AvatarId = newAvatar.Id;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            await this.imagesService.DeleteImageAsync(oldAvatarId);

            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
        }

        public bool IsUserProfileUpdated(string userId)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.Id == userId);

            return user.IsUserProfileUpdated;
        }

        public async Task DeleteAvatar(string id)
        {
            var user = this.usersRepository.All().FirstOrDefault(u => u.Id == id);
            var avatarId = user.AvatarId;
            user.AvatarId = null;

            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
            await this.imagesService.DeleteImageAsync(avatarId);
            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.ProfileUpdated,
                        EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
        }

        public string GetArenaAdminIdByArenaId(int arenaId)
        {
            return this.usersRepository.All()
                .Where(u => u.AdministratingArena.Id == arenaId)
                .Select(u => u.Id)
                .FirstOrDefault();
        }
    }
}
