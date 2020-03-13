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
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.UsersProfile;

    public class UsersProfileService : IUsersProfileService
    {
        private readonly IDeletableEntityRepository<UserProfile> userProfilesRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;

        private readonly string avatarImageSizing = "w_200,h_200,c_crop,g_face,r_max/w_200/";
        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_qjeerp.png";
        private readonly string noAvatarId = "noAvatar";

        public UsersProfileService(
            IDeletableEntityRepository<UserProfile> userProfilesRepository,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IImagesService imagesService)
        {
            this.userProfilesRepository = userProfilesRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
        }

        public async Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId)
        {
            var avatarId = await this.imagesService.CreateAsync(inputModel.AvatarImage, this.noAvatarUrl);

            if (avatarId == null)
            {
                avatarId = this.noAvatarId;
            }

            var profile = new UserProfile
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Age = inputModel.Age,
                FaceBookAccount = inputModel.FaceBookAccount,
                Gender = inputModel.Gender,
                PhoneNumber = inputModel.PhoneNumber,
                Status = inputModel.Status,
                SportId = inputModel.Sport,
                CityId = inputModel.City,
                AvatarId = avatarId,
                ApplicationUserId = userId,
                Occupation = inputModel.Occupation,
            };

            await this.userProfilesRepository.AddAsync(profile);
            await this.userProfilesRepository.SaveChangesAsync();

            return profile.Id;
        }

        public UserProfileDetailsViewModel GetDetails(string id)
        {
            var imagePathPrefix = this.imagesService.ConstructUrlPrefix(this.avatarImageSizing);

            var viewModel = this.userProfilesRepository
                .All()
                .Where(up => up.Id == id)
                .Select(up => new UserProfileDetailsViewModel
                {
                    UserProfileId = id,
                    FullName = up.FirstName + " " + up.LastName,
                    FavoriteSport = up.Sport.Name,
                    Location = up.City.Name + ", " + up.City.Country.Name,
                    Age = up.Age,
                    Gender = up.Gender.ToString(),
                    FaceBookAccount = up.FaceBookAccount,
                    PhoneNumber = up.PhoneNumber,
                    Status = up.Status.ToString(),
                    AvatarImageId = up.AvatarId,
                    AvatarImageUrl = imagePathPrefix + up.Avatar.Url,
                    OrginizedEventsCount = up.ApplicationUser.Events.Count,
                })
                .FirstOrDefault();

            return viewModel;
        }

        public UserProfileEditViewModel GetDetailsForEdit(string id)
        {
            var viewModel = this.userProfilesRepository
                .All()
                .Where(up => up.Id == id)
                .Select(up => new UserProfileEditViewModel
                {
                    Id = id,
                    FirstName = up.FirstName,
                    LastName = up.LastName,
                    Sport = up.Sport.Id,
                    Age = up.Age,
                    FaceBookAccount = up.FaceBookAccount,
                    Gender = up.Gender,
                    PhoneNumber = up.PhoneNumber,
                    Occupation = up.Occupation,
                    City = up.City.Id,
                    Country = up.City.Country.Id,
                    Status = up.Status,
                }).FirstOrDefault();

            viewModel.Countries = this.countriesService.GetAll();
            viewModel.Cities = this.citiesService.GetCitiesSelectList(viewModel.Country);
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
            userProfile.SportId = inputModel.Sport;
            userProfile.Status = inputModel.Status;
            userProfile.CityId = inputModel.City;

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
    }
}
