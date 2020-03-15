namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.UsersProfile;
    using Microsoft.Extensions.Configuration;

    public class UsersProfileService : IUsersProfileService
    {
        private readonly IDeletableEntityRepository<UserProfile> userProfilesRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;
        private readonly IConfiguration configuration;
        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";
        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_qjeerp.png";
        private readonly string noAvatarId = "noAvatar";

        public UsersProfileService(
            IDeletableEntityRepository<UserProfile> userProfilesRepository,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IImagesService imagesService,
            IConfiguration configuration)
        {
            this.userProfilesRepository = userProfilesRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId)
        {
            var avatarId = await this.imagesService.CreateAsync(inputModel.AvatarImage, this.noAvatarUrl);

            if (avatarId == null)
            {
                avatarId = this.noAvatarId;
            }

            var profile = inputModel.To<UserProfile>();

            profile.ApplicationUserId = userId;
            profile.AvatarId = avatarId;

            await this.userProfilesRepository.AddAsync(profile);
            await this.userProfilesRepository.SaveChangesAsync();

            return profile.Id;
        }

        public UserProfileDetailsViewModel GetDetails(string id)
        {
            var imagePathPrefix = this.imagesService.ConstructUrlPrefix(this.avatarImageSizing);

            var query = this.userProfilesRepository
                .All()
                .Where(up => up.Id == id);

            var viewModel = query.To<UserProfileDetailsViewModel>().FirstOrDefault();
            viewModel.AvatarUrl = imagePathPrefix + viewModel.AvatarUrl;
            return viewModel;
        }

        public UserProfileEditViewModel GetDetailsForEdit(string id)
        {
            var query = this.userProfilesRepository
                .All()
                .Where(up => up.Id == id);

            var viewModel = query.To<UserProfileEditViewModel>().FirstOrDefault();

            viewModel.Countries = this.countriesService.GetAll();
            viewModel.Cities = this.citiesService.GetCitiesSelectList(viewModel.CityCountryId);
            viewModel.Sports = this.sportsService.GetAll();

            return viewModel;
        }

        public async Task UpdateAsync(UserProfileEditViewModel inputModel)
        {
            var userProfile = this.userProfilesRepository
                .All()
                .Where(up => up.Id == inputModel.Id)
                .FirstOrDefault();

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

            this.userProfilesRepository.Update(userProfile);
            await this.userProfilesRepository.SaveChangesAsync();
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
            var avatarUrl = this.userProfilesRepository.
                All()
                .Where(up => up.ApplicationUser.Id == userId)
                .Select(up => up.Avatar.Url)
                .FirstOrDefault();

            return this.imagePathPrefix + avatarUrl;
        }
    }
}
