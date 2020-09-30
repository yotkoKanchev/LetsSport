namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Data.Users;
    using LetsSport.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class UsersServiceTests : BaseServiceTests
    {
        private const string TestImagePath = "Test.jpg";
        private const string InvalidTestImagePath = "Test.docx";
        private const string TestImageContentType = "image/jpg";
        private string userId;

        public UsersServiceTests()
        {
            this.userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
        }

        private IUsersService Service => this.ServiceProvider.GetRequiredService<IUsersService>();

        [Fact]
        public async Task GetAllByEventIdAsyncReturnsCorrectNumer()
        {
            var result = await this.Service.GetAllByEventIdAsync(1);
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateAsyncUpdatesDataCorrectly()
        {
            var inputModel = new UserUpdateInputModel
            {
                Age = 20,
                FirstName = "Test",
                LastName = "Testov",
                FaceBookAccount = "https://facebookAccount.com",
                Gender = Gender.Male,
                Occupation = "student",
            };

            await this.Service.UpdateAsync(inputModel, this.userId, "email", "username");
            var user = this.DbContext.ApplicationUsers.Where(u => u.Id == this.userId).First();

            Assert.Equal(20, user.Age);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("Testov", user.LastName);
            Assert.Equal("https://facebookAccount.com", user.FaceBookAccount);
            Assert.Equal("student", user.Occupation);
        }

        [Fact]
        public async Task UpdateAsyncUpdatesAvatarImage()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };
            var inputModel = new UserUpdateInputModel
            {
                AvatarImage = file,
            };

            await this.Service.UpdateAsync(inputModel, this.userId, "email", "username");
            var image = this.DbContext.Images.Where(i => i.User.Id == this.userId).First();
            Assert.NotNull(image.Url);
            Assert.StartsWith("v", image.Url);
        }

        [Fact]
        public async Task GetDetailsByIdAsyncReturnsCorrectDetails()
        {
            var viewModel = await this.Service.GetDetailsByIdAsync<UserDetailsViewModel>(this.userId);
            Assert.Equal("tester", viewModel.UserName);
            Assert.Equal("testCity, testCountry", viewModel.Location);
        }

        [Fact]
        public async Task GetDetailsByIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsByIdAsync<UserDetailsViewModel>("id"));
        }

        [Fact]
        public async Task GetDetailsByIdAsyncThrowsIfIdIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsByIdAsync<UserDetailsViewModel>(null));
        }

        [Fact]
        public void GetUserAvatarUrlRerunsNoAvatarUrlIfNoAvatar()
        {
            var result = this.Service.GetUserAvatarUrl(this.userId);
            Assert.Equal(GlobalConstants.NoAvatarImagePath, result);
        }

        [Fact]
        public async Task GetUserAvatarUrlRerunsValidAvatarUrl()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };
            var inputModel = new UserUpdateInputModel
            {
                AvatarImage = file,
            };

            await this.Service.UpdateAsync(inputModel, this.userId, "email", "username");
            var result = this.Service.GetUserAvatarUrl(this.userId);
            var imageUrl = this.DbContext.Images
                .Where(i => i.User.Id == this.userId).Select(i => i.Url).First();

            Assert.Contains(imageUrl, result);
        }

        [Fact]
        public async Task ChangeAvatarAsyncWorksCorrect()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.ChangeAvatarAsync(this.userId, file);
            var result = this.Service.GetUserAvatarUrl(this.userId);
            var imageUrl = this.DbContext.Images
                .Where(i => i.User.Id == this.userId).Select(i => i.Url).First();

            Assert.Contains(imageUrl, result);
        }

        [Fact]
        public async Task DeleteAvatarRemoveAvatarImageFromDb()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.ChangeAvatarAsync(this.userId, file);
            await this.Service.DeleteAvatar(this.userId);
            var result = this.DbContext.Images.FirstOrDefault(i => i.User.Id == this.userId);
            Assert.Null(result);
        }

        [Fact]
        public async Task IsProfileUpdatedAsyncReturnsFalseIfNotUpdated()
        {
            Assert.False(await this.Service.IsProfileUpdatedAsync(this.userId));
        }

        [Fact]
        public async Task IsProfileUpdatedAsyncReturnsTrueIfUpdated()
        {
            var inputModel = new UserUpdateInputModel
            {
                Age = 20,
                FirstName = "Test",
                LastName = "Testov",
                Status = UserStatus.ProposalOpen,
            };

            await this.Service.UpdateAsync(inputModel, this.userId, "email", "username");
            Assert.True(await this.Service.IsProfileUpdatedAsync(this.userId));
        }

        [Fact]
        public async Task GetAllUsersDetailsForIvitationAsyncReturnsCorrectViewModel()
        {
            var results = await this.Service.GetAllUsersDetailsForIvitationAsync(1, 1);
            foreach (var result in results)
            {
                Assert.Equal("tester", result.Username);
                Assert.Equal("user@abv.bg", result.Email);
            }
        }

        [Fact]
        public void SetAvatarImageSetsCorrectPrefix()
        {
            var result = this.Service.SetAvatarImage("image");
            Assert.Contains(this.Configuration["Cloudinary:ApiName"], result);
            Assert.StartsWith(string.Format(GlobalConstants.CloudinaryPrefix, this.Configuration["Cloudinary:ApiName"]), result);
        }

        [Fact]
        public async Task GetUserNameByUserIdAsyncReturnsCorrectName()
        {
            var result = await this.Service.GetUserNameByUserIdAsync(this.userId);
            Assert.Equal("tester", result);
        }

        [Fact]
        public async Task GetUserNameByUserIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetUserNameByUserIdAsync("id"));
        }

        [Fact]
        public async Task BlockUserAsyncRemoveUser()
        {
            await this.Service.BlockUserAsync(this.userId);
            var user = this.DbContext.ApplicationUsers.Where(u => u.Id == this.userId).FirstOrDefault();
            Assert.Null(user);
            Assert.Empty(this.DbContext.EventsUsers);
        }

        [Fact]
        public async Task BlockUserAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.BlockUserAsync("id"));
        }

        [Fact]
        public async Task IsUserHasArenaAsyncRerunsTrueWithValidArguments()
        {
            var result = await this.Service.IsUserHasArenaAsync(this.userId);
            Assert.True(result);
        }

        [Fact]
        public async Task IsUserHasArenaAsyncRerunsFalseIValidArguments()
        {
            var user = new ApplicationUser
            {
                Email = "testEmail",
                PasswordHash = "dasdadas",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(user);
            this.DbContext.SaveChanges();

            var newUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();

            var result = await this.Service.IsUserHasArenaAsync(newUserId);
            Assert.False(result);
        }

        [Fact]
        public async Task IsUserHasArenaAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.IsUserHasArenaAsync("id"));
        }
    }
}
