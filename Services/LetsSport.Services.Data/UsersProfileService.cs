﻿namespace LetsSport.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.UsersProfile;

    public class UsersProfileService : IUsersProfileService
    {
        private readonly IDeletableEntityRepository<UserProfile> userProfilesRepository;
        private readonly ICitiesService citiesService;
        private readonly IImagesService imagesService;
        private readonly string avatarImageSizing = "w_400,h_400,c_crop,g_face,r_max/w_200/";

        public UsersProfileService(
            IDeletableEntityRepository<UserProfile> userProfilesRepository,
            ICitiesService citiesService,
            IImagesService imagesService)
        {
            this.userProfilesRepository = userProfilesRepository;
            this.citiesService = citiesService;
            this.imagesService = imagesService;
        }

        public async Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId)
        {
            var genderType = (Gender)Enum.Parse(typeof(Gender), inputModel.Gender);
            var favoriteSport = (SportType)Enum.Parse(typeof(SportType), inputModel.FavoriteSport);
            var statusType = (UserStatus)Enum.Parse(typeof(UserStatus), inputModel.Status);

            var cityId = await this.citiesService.GetCityIdAsync(inputModel.City, inputModel.Country);
            var avatarId = await this.imagesService.CreateAsync(inputModel.AvatarUrl);

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
                AvatarId = avatarId,
                FavoriteSport = favoriteSport,
                CityId = cityId,
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
                .AllAsNoTracking()
                .Where(up => up.Id == id)
                .Select(up => new UserProfileDetailsViewModel
                {
                    Id = id,
                    FullName = up.FirstName + " " + up.LastName,
                    FavoriteSport = up.FavoriteSport.ToString(),
                    Location = up.City.Name + ", " + up.City.Country.Name,
                    Age = up.Age,
                    Gender = up.Gender.ToString(),
                    FaceBookAccount = up.FaceBookAccount,
                    PhoneNumber = up.PhoneNumber,
                    Status = up.Status.ToString(),
                    AvatarUrl = imagePathPrefix + up.Avatar.Url,
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
                    FavoriteSport = up.FavoriteSport.ToString(),
                    Age = up.Age,
                    FacebookAccount = up.FaceBookAccount,
                    Gender = up.Gender.ToString(),
                    PhoneNumber = up.PhoneNumber,
                    Ocupation = up.Occupation,
                    City = up.City.Name,
                    Country = up.City.Country.Name,
                    Status = up.Status.ToString(),
                }).FirstOrDefault();

            return viewModel;
        }

        public async Task UpdateAsync(UserProfileEditInputModel inputModel)
        {
            var userCity = this.userProfilesRepository
                .All()
                .Where(up => up.Id == inputModel.Id)
                .Select(up => up.City.Name)
                .FirstOrDefault();

            var userProfile = this.userProfilesRepository
                .All()
                .Where(up => up.Id == inputModel.Id)
                .FirstOrDefault();

            userProfile.FirstName = inputModel.FirstName;
            userProfile.LastName = inputModel.LastName;
            userProfile.Age = inputModel.Age;
            userProfile.PhoneNumber = inputModel.PhoneNumber;
            userProfile.FaceBookAccount = inputModel.FaceBookAccount;
            userProfile.ModifiedOn = DateTime.UtcNow;
            userProfile.Occupation = inputModel.Occupation;

            if (userProfile.FavoriteSport.ToString() != inputModel.FavoriteSport)
            {
                userProfile.FavoriteSport = (SportType)Enum.Parse(typeof(SportType), inputModel.FavoriteSport);
            }

            if (userProfile.Gender.ToString() != inputModel.Gender)
            {
                userProfile.Gender = (Gender)Enum.Parse(typeof(Gender), inputModel.Gender);
            }

            if (userProfile.Status.ToString() != inputModel.Status)
            {
                userProfile.Status = (UserStatus)Enum.Parse(typeof(UserStatus), inputModel.Status);
            }

            if (userCity != inputModel.City)
            {
                var newCityId = await this.citiesService.GetCityIdAsync(inputModel.City, inputModel.Country);
                userProfile.CityId = newCityId;
            }

            this.userProfilesRepository.Update(userProfile);
            await this.userProfilesRepository.SaveChangesAsync();
        }
    }
}
