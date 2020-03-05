namespace LetsSport.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.UsersProfile;
    using Microsoft.Extensions.Configuration;

    public class UsersProfileService : IUsersProfileService
    {
        private readonly IDeletableEntityRepository<UserProfile> userProfilesRepository;
        private readonly Cloudinary cloudinary;
        private readonly IConfiguration configuration;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";
        private readonly string mainImageSizing = "w_100,h_100,c_scale,r_50,bo_1px_solid_blue/";

        public UsersProfileService(
            IDeletableEntityRepository<UserProfile> userProfilesRepository,
            Cloudinary cloudinary,
            IConfiguration configuration)
        {
            this.userProfilesRepository = userProfilesRepository;
            this.cloudinary = cloudinary;
            this.configuration = configuration;
        }

        public async Task<string> CreateUserProfile(UserProfileCreateInputModel inputModel, string userId)
        {
            var imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:AppName"]);

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
            };

            await this.userProfilesRepository.AddAsync(profile);
            await this.userProfilesRepository.SaveChangesAsync();

            return profile.Id;
        }
    }
}
