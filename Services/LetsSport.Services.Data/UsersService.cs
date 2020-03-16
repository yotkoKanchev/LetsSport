namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.Extensions.Configuration;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;
        private readonly IConfiguration configuration;
        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_300/";
        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_ppq2gm.png";
        private readonly string noAvatarId = "noAvatar";

        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IImagesService imagesService,
            IConfiguration configuration)
        {
            this.eventsUsersRepository = eventsUsersRepository;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
            this.usersRepository = usersRepository;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<string> FillAdditionalUserInfo(UserUpdateInputModel inputModel, string userId)
        {
            var avatarId = await this.imagesService.CreateAsync(inputModel.AvatarImage, this.noAvatarUrl);

            if (avatarId == null)
            {
                avatarId = this.noAvatarId;
            }

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
            profile.AvatarId = avatarId;
            profile.SportId = inputModel.SportId;

            this.usersRepository.Update(profile);
            await this.usersRepository.SaveChangesAsync();

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

        public UserDetailsViewModel GetDetails(string id)
        {
            var imagePathPrefix = this.imagesService.ConstructUrlPrefix(this.avatarImageSizing);

            var query = this.usersRepository
                .All()
                .Where(up => up.Id == id);

            var viewModel = query.To<UserDetailsViewModel>().FirstOrDefault();
            viewModel.AvatarUrl = imagePathPrefix + viewModel.AvatarUrl;
            return viewModel;
        }

        public UserEditViewModel GetDetailsForEdit(string id)
        {
            var query = this.usersRepository
                .All()
                .Where(up => up.Id == id);

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

            return this.imagePathPrefix + this.avatarImageSizing + avatarUrl;
        }
    }
}
