namespace LetsSport.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.UsersProfile;
    using Microsoft.Extensions.Configuration;

    public class UsersProfileService : IUsersProfileService
    {
        private readonly IDeletableEntityRepository<UserProfile> userProfilesRepository;
        private readonly Cloudinary cloudinary;
        private readonly IConfiguration configuration;
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_200/";

        public UsersProfileService(
            IDeletableEntityRepository<UserProfile> userProfilesRepository,
            Cloudinary cloudinary,
            IConfiguration configuration,
            ICitiesService citiesService,
            ICountriesService countriesService)
        {
            this.userProfilesRepository = userProfilesRepository;
            this.cloudinary = cloudinary;
            this.configuration = configuration;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
        }

        public async Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId)
        {
            var imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:AppName"]);
            var cityId = await this.citiesService.GetCityIdAsync(inputModel.City, inputModel.Country);
            var countryId = await this.countriesService.GetCountryIdAsync(inputModel.Country);

            var genderType = (Gender)Enum.Parse(typeof(Gender), inputModel.Gender);
            var statusType = (UserStatus)Enum.Parse(typeof(UserStatus), inputModel.Status);
            var avatarImage = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, inputModel.AvatarUrl);
            avatarImage = avatarImage.Replace(imagePathPrefix, string.Empty);

            var profile = new UserProfile
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                ApplicationUserId = userId,
                Age = inputModel.Age,
                FaceBookAccount = inputModel.FaceBookAccount,
                Gender = genderType,
                PhoneNumber = inputModel.PhoneNumber,
                Status = statusType,
                AvatarUrl = avatarImage,
                CityId = cityId,
                CountryId = countryId,
            };

            await this.userProfilesRepository.AddAsync(profile);
            await this.userProfilesRepository.SaveChangesAsync();

            return profile.Id;
        }

        public UserProfileDetailsViewModel GetDetails(string id)
        {
            var imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:AppName"]);

            var viewModel = this.userProfilesRepository
                .All()
                .Where(up => up.Id == id)
                .Select(up => new UserProfileDetailsViewModel
                {
                    Id = id,
                    FullName = up.FirstName + " " + up.LastName,
                    Location = up.City.Name + ", " + up.City.Country.Name,
                    Age = up.Age,
                    Gender = up.Gender.ToString(),
                    FaceBookAccount = up.FaceBookAccount,
                    PhoneNumber = up.PhoneNumber,
                    Status = up.Status.ToString(),
                    AvatarUrl = imagePathPrefix + this.avatarImageSizing + up.AvatarUrl,
                })
                .FirstOrDefault();

            return viewModel;
        }
    }
}
