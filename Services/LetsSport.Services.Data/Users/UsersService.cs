﻿namespace LetsSport.Services.Data.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.Users;
    using LetsSport.Services.Data.Cities;
    using LetsSport.Services.Data.Cloudinary;
    using LetsSport.Services.Data.Countries;
    using LetsSport.Services.Data.Images;
    using LetsSport.Services.Data.Sports;
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
        private readonly ICitiesService citiesService;
        private readonly ICountriesService countriesService;
        private readonly ISportsService sportsService;
        private readonly IImagesService imagesService;
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly IEmailSender emailSender;
        private readonly string imagePathPrefix;

        public UsersService(
            ICitiesService citiesService,
            ICountriesService countriesService,
            ISportsService sportsService,
            IImagesService imagesService,
            ICloudinaryHelper cloudinaryHelper,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<EventUser> eventsUsersRepository,
            IEmailSender emailSender)
        {
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.sportsService = sportsService;
            this.imagesService = imagesService;
            this.usersRepository = usersRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.emailSender = emailSender;
            this.imagePathPrefix = cloudinaryHelper.GetPrefix();
        }

        public async Task<IEnumerable<EventUserViewModel>> GetAllByEventIdAsync(int id)
            => await this.eventsUsersRepository
                .All()
                .Where(ev => ev.EventId == id)
                .OrderBy(ev => ev.User.UserName)
                .To<EventUserViewModel>()
                .ToListAsync();

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

        public async Task<T> GetDetailsByIdAsync<T>(string id)
        {
            var query = this.GetUserByIdAsIQueryable(id);
            var viewModel = query.To<T>();

            return await viewModel.FirstOrDefaultAsync();
        }

        public async Task<UserUpdateInputModel> GetDetailsForEditAsync(string id, (string CityName, string CountryName) location)
        {
            var query = this.GetUserByIdAsIQueryable(id);
            var countryId = await this.countriesService.GetIdAsync(location.CountryName);
            var viewModel = await query.To<UserUpdateInputModel>().FirstOrDefaultAsync();
            var cityName = await this.citiesService.GetNameByIdAsync(viewModel.CityId) ?? location.CityName;
            viewModel.Countries = await this.countriesService.GetAllAsSelectListAsync();
            viewModel.Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId);
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();
            viewModel.CountryId = countryId;
            viewModel.CityId = await this.citiesService.GetIdAsync(cityName, countryId);

            return viewModel;
        }

        public string GetUserAvatarUrl(string userId)
        {
            var avatarUrl = this.usersRepository
                .All()
                .Where(u => u.Id == userId)
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
                await this.imagesService.DeleteByIdAsync(oldAvatarId);
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

            if (avatarId != null)
            {
                user.AvatarId = null;
                this.usersRepository.Update(user);
                await this.usersRepository.SaveChangesAsync();
                await this.imagesService.DeleteByIdAsync(avatarId);
                await this.emailSender.SendEmailAsync(
                            user.Email,
                            EmailSubjectConstants.ProfileUpdated,
                            EmailHtmlMessages.GetUpdateProfileHtml(user.UserName));
            }
        }

        public async Task<IEnumerable<EmailUserInfo>> GetAllUsersDetailsForIvitationAsync(int sportId, int arenaCityId)
            => await this.usersRepository
                .All()
                .Where(u => u.CityId == arenaCityId)
                .Where(u => u.Sport.Id == sportId)
                .Where(u => u.Status == UserStatus.ProposalOpen)
                .Select(u => new EmailUserInfo
                {
                    Email = u.Email,
                    Username = u.UserName,
                })
                .ToListAsync();

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
            => await this.GetUserByIdAsIQueryable(reportedUserId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

        public async Task BlockUserAsync(string userId)
        {
            var user = await this.GetUserByIdAsIQueryable(userId).FirstAsync();
            user.IsDeleted = true;
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            var eventUsers = await this.eventsUsersRepository
                .All()
                .Where(eu => eu.UserId == userId)
                .ToListAsync();

            foreach (var eu in eventUsers)
            {
                this.eventsUsersRepository.Delete(eu);
            }

            await this.eventsUsersRepository.SaveChangesAsync();
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
            var user = this.usersRepository
                .All()
                .Where(u => u.Id == userId);

            if (!user.Any())
            {
                throw new ArgumentException(string.Format(UserInvalidIdErrorMessage, userId));
            }

            return user;
        }
    }
}
