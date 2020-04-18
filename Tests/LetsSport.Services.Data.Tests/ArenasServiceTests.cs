namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class ArenasServiceTests : BaseServiceTests
    {
        private const string TestImagePath = "Test.jpg";
        private const string InvalidTestImagePath = "Test.docx";
        private const string TestImageContentType = "image/jpg";
        private string userId;

        public ArenasServiceTests()
        {
            this.userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            var secondCountry = new Country
            {
                Name = "secondCountry",
            };
            this.DbContext.Countries.Add(secondCountry);
            this.DbContext.SaveChanges();

            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 2,
            };

            this.DbContext.Cities.Add(secondCity);
            this.DbContext.SaveChanges();

            var secondSport = new Sport
            {
                Name = "secondSport",
                Image = "https://newsport.jpg",
            };

            this.DbContext.Sports.Add(secondSport);
            this.DbContext.SaveChanges();

            var secondUser = new ApplicationUser
            {
                Email = "secondEmail",
                PasswordHash = "sssdddfff",
                CityId = 2,
                CountryId = 2,
                SportId = 2,
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            this.DbContext.SaveChanges();

            var secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();

            var secondArena = new Arena
            {
                Name = "newArena",
                ArenaAdminId = secondUserId,
                SportId = 2,
                CityId = 2,
                CountryId = 2,
                PricePerHour = 200,
                Status = ArenaStatus.Active,
                PhoneNumber = "0777777777",
            };

            this.DbContext.Arenas.Add(secondArena);
            this.DbContext.SaveChanges();
        }

        private IArenasService Service => this.ServiceProvider.GetRequiredService<IArenasService>();

        [Fact]
        public async Task GetAllInCityAsyncReaturnsAllArenasInCity()
        {
            var arenas = await this.Service.GetAllInCityAsync(1);
            Assert.Single(arenas);

            foreach (var arena in arenas)
            {
                Assert.Equal("testArena", arena.Name);
                Assert.Equal(20, arena.PricePerHour);
                Assert.Equal(1, arena.SportId);
                Assert.Equal(1, arena.EventsCount);
            }
        }

        [Fact]
        public async Task GetCountInCityAsyncReturnsCorrectNumber()
        {
            var result = await this.Service.GetCountInCityAsync(1);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetAllActiveInCitySelectListAsyncReturnsOnlyActiveArenas()
        {
            var secondUser = new ApplicationUser
            {
                Email = "email@abv.bg",
                PasswordHash = "dakdjasdjd",
                CityId = 1,
                CountryId = 1,
                UserName = "tester1",
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            await this.DbContext.SaveChangesAsync();
            var secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();
            var secondArena = new Arena
            {
                Name = "secondArena",
                Status = ArenaStatus.Inactive,
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                ArenaAdminId = secondUserId,
                PricePerHour = 20,
            };

            this.DbContext.Arenas.Add(secondArena);
            await this.DbContext.SaveChangesAsync();
            var arenas = await this.Service.GetAllActiveInCitySelectListAsync(1);
            Assert.Single(arenas);
        }

        [Fact]
        public async Task GetByIdAsyncReturnsCorrectRecord()
        {
            var model = await this.Service.GetByIdAsync<DetailsViewModel>(1);
            Assert.Null(model.Address);
            Assert.Null(model.Description);
            Assert.Equal(1, model.CountryId);
            Assert.Equal("testCity", model.CityName);
            Assert.Equal("testCountry", model.CountryName);
            Assert.Equal("testSport", model.SportName);
            Assert.Equal(ArenaStatus.Active, model.Status);
        }

        [Fact]
        public async Task GetIdByAdminIdAsyncReturnsCorrectId()
        {
            var id = await this.Service.GetIdByAdminIdAsync(this.userId);
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task GetIdByAdminIdThrowsIfInvalidUserId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetIdByAdminIdAsync("id"));
        }

        [Fact]
        public async Task CreateAsyncAddArenaInDb()
        {
            var inputModel = new ArenaCreateInputModel
            {
                Name = "newArena",
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                PricePerHour = 20,
                Status = ArenaStatus.Active,
                PhoneNumber = "0877777777",
            };

            await this.Service.CreateAsync(inputModel, this.userId, "email", "username");
            Assert.Equal(3, this.DbContext.Arenas.Count());
        }

        [Fact]
        public async Task GetDetailsAsyncReturnsCorrectResult()
        {
            var result = await this.Service.GetDetailsAsync<ArenaDetailsViewModel>(1);
            Assert.Null(result.Address);
            Assert.Null(result.Description);
            Assert.Null(result.Email);
            Assert.Null(result.WebUrl);
            Assert.Equal("testArena", result.Name);
            Assert.Equal(this.userId, result.ArenaAdminId);
            Assert.Equal("tester", result.ArenaAdminUserName);
            Assert.Equal(1, result.EventsCount);
            Assert.Equal(20, result.PricePerHour);
            Assert.Equal(ArenaStatus.Active.ToString(), result.Status);
        }

        [Fact]
        public async Task GetDetailsAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsAsync<ArenaDetailsViewModel>(11));
        }

        [Fact]
        public async Task GetDetailsForEditAsyncReturnsCorrectDetails()
        {
            var result = await this.Service.GetDetailsForEditAsync(1);
            Assert.Null(result.Address);
            Assert.Null(result.Description);
            Assert.Null(result.Email);
            Assert.Null(result.WebUrl);
            Assert.Equal("testArena", result.Name);
            Assert.Equal(this.userId, result.ArenaAdminId);
            Assert.Equal(20, result.PricePerHour);
            Assert.Equal(ArenaStatus.Active, result.Status);
        }

        [Fact]
        public async Task GetDetailsForEditAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsForEditAsync(11));
        }

        [Fact]
        public async Task UpdateAsyncUpdatesArenaCorrectly()
        {
            var arena = new Arena
            {
                Name = "newArena",
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                PricePerHour = 20,
                Status = ArenaStatus.Active,
                PhoneNumber = "0877777777",
            };

            var sport = new Sport
            {
                Name = "secondSport",
                Image = "https://newsport.jpg",
            };

            await this.DbContext.Arenas.AddAsync(arena);
            await this.DbContext.Sports.AddAsync(sport);
            await this.DbContext.SaveChangesAsync();

            var inputModel = new ArenaEditViewModel
            {
                Id = 2,
                Name = "editedName",
                Address = "editedAddress",
                Email = "editedEmail",
                WebUrl = "editedWeb",
                SportId = 2,
            };

            await this.Service.UpdateAsync(inputModel);

            var arenaFromDb = this.DbContext.Arenas.FirstOrDefault(a => a.Id == 2);

            Assert.Equal("editedName", arenaFromDb.Name);
            Assert.Equal("editedAddress", arenaFromDb.Address);
            Assert.Equal("editedEmail", arenaFromDb.Email);
            Assert.Equal("editedWeb", arenaFromDb.WebUrl);
            Assert.Equal(2, arenaFromDb.SportId);
        }

        [Fact]
        public async Task FilterAsyncByCountryReturnsCorrectResult()
        {
            var result = await this.Service.FilterAsync(2, null, null);
            Assert.Null(result.CityId);
            Assert.Null(result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("secondCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task FilterAsyncByCityReturnsCorrectResult()
        {
            var result = await this.Service.FilterAsync(2, null, 2);
            Assert.Equal(2, result.CityId);
            Assert.Null(result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("secondCity, secondCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task FilterAsyncBySportReturnsCorrectResult()
        {
            var result = await this.Service.FilterAsync(2, 2, null);
            Assert.Null(result.CityId);
            Assert.Equal(2, result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("secondCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task FilterAsyncBySportAndCityReturnsCorrectResult()
        {
            var result = await this.Service.FilterAsync(2, 2, 2);
            Assert.Equal(2, result.CityId);
            Assert.Equal(2, result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("secondCity, secondCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public void SetMainImageIfNoImage()
        {
            var result = this.Service.SetMainImage(null);
            Assert.Equal(GlobalConstants.DefaultMainImagePath, result);
        }

        [Fact]
        public void SetMainImageIfWithImage()
        {
            var result = this.Service.SetMainImage("image");
            Assert.Contains(this.Configuration["Cloudinary:ApiName"], result);
            Assert.StartsWith(string.Format(GlobalConstants.CloudinaryPrefix, this.Configuration["Cloudinary:ApiName"]), result);
        }

        [Fact]
        public async Task ChangeMainImageAsyncUpdateImageCorrectly()
        {
            Assert.Null(this.DbContext.Arenas
                .Where(a => a.Id == 2).Select(a => a.MainImageId).FirstOrDefault());

            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.ChangeMainImageAsync(2, file);

            Assert.NotNull(this.DbContext.Arenas
                .Where(a => a.Id == 2).Select(a => a.MainImageId).FirstOrDefault());
        }

        [Fact]
        public async Task ChangeMainImageThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.ChangeMainImageAsync(11, new Mock<IFormFile>().Object));
        }

        [Fact]
        public async Task DeleteMainImageAsyncRemoveImage()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.ChangeMainImageAsync(2, file);
            Assert.NotNull(this.DbContext.Arenas
                .Where(a => a.Id == 2).Select(a => a.MainImageId).FirstOrDefault());

            await this.Service.DeleteMainImageAsync(2);
            Assert.Null(this.DbContext.Arenas
                .Where(a => a.Id == 2).Select(a => a.MainImageId).FirstOrDefault());
        }

        [Fact]
        public async Task DeleteMainImageAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.DeleteMainImageAsync(11));
        }

        [Fact]
        public async Task GetImagesByIdAsyncReturnsImagesCorrectly()
        {
            var result = await this.Service.GetImagesByIdAsync(2);
            Assert.Empty(result.Images);

            var image = new Image
            {
                ArenaId = 2,
                Url = "imageUrl",
            };

            var arena = this.DbContext.Arenas.First(a => a.Id == 2);
            arena.Images.Add(image);
            await this.DbContext.SaveChangesAsync();

            result = await this.Service.GetImagesByIdAsync(2);
            Assert.Single(result.Images);
        }

        [Fact]
        public async Task GetImagesByIdAsyncThrowsWithInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetImagesByIdAsync(11));
        }

        [Fact]
        public async Task AddImagesAsyncAddsImageInDb()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            List<IFormFile> imgs = new List<IFormFile> { file };

            await this.Service.AddImagesAsync(imgs, 2);
            var arena = this.DbContext.Arenas.First(a => a.Id == 2);
            Assert.Single(arena.Images);
        }

        [Fact]
        public async Task AddImagesAsyncThrowsIfInvalidId()
        {
            var mockListWithIFormFiles = new Mock<List<IFormFile>>();
            await Assert.ThrowsAsync<ArgumentException>(()
                    => this.Service.AddImagesAsync(mockListWithIFormFiles.Object, 11));
        }

        [Fact]
        public async Task GetImageUrslByIdAsyncReturnsCorrectUrls()
        {
            var image = new Image
            {
                ArenaId = 2,
                Url = "imageUrl",
            };

            var arena = this.DbContext.Arenas.First(a => a.Id == 2);
            arena.Images.Add(image);
            await this.DbContext.SaveChangesAsync();

            var urls = await this.Service.GetImageUrslByIdAsync(2);

            foreach (var url in urls)
            {
                Assert.Contains(this.Configuration["Cloudinary:ApiName"], url);
                Assert.StartsWith(string.Format(GlobalConstants.CloudinaryPrefix, this.Configuration["Cloudinary:ApiName"]), url);
            }
        }

        [Fact]
        public async Task GetAllInCitySelectListAsyncReturnAllStatused()
        {
            var secondUser = new ApplicationUser
            {
                Email = "email@abv.bg",
                PasswordHash = "dakdjasdjd",
                CityId = 1,
                CountryId = 1,
                UserName = "tester1",
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            await this.DbContext.SaveChangesAsync();
            var secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();
            var secondArena = new Arena
            {
                Name = "secondArena",
                Status = ArenaStatus.Inactive,
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                ArenaAdminId = secondUserId,
                PricePerHour = 20,
            };

            this.DbContext.Arenas.Add(secondArena);
            await this.DbContext.SaveChangesAsync();

            var arenas = await this.Service.GetAllInCitySelectListAsync(1);
            Assert.Equal(2, arenas.Count());
        }

        [Fact]
        public async Task GetAllInCountryAsyncReturnAllStatused()
        {
            var secondUser = new ApplicationUser
            {
                Email = "email@abv.bg",
                PasswordHash = "dakdjasdjd",
                CityId = 1,
                CountryId = 1,
                UserName = "tester1",
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            await this.DbContext.SaveChangesAsync();
            var secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();
            var secondArena = new Arena
            {
                Name = "secondArena",
                Status = ArenaStatus.Inactive,
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                ArenaAdminId = secondUserId,
                PricePerHour = 20,
            };

            this.DbContext.Arenas.Add(secondArena);
            await this.DbContext.SaveChangesAsync();

            var arenas = await this.Service.GetAllInCountryAsync<InfoViewModel>(1);
            Assert.Equal(2, arenas.Count());
        }

        [Fact]
        public async Task GetCountInCountryAsyncAllStatused()
        {
            var secondUser = new ApplicationUser
            {
                Email = "email@abv.bg",
                PasswordHash = "dakdjasdjd",
                CityId = 1,
                CountryId = 1,
                UserName = "tester1",
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            await this.DbContext.SaveChangesAsync();
            var secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();
            var secondArena = new Arena
            {
                Name = "secondArena",
                Status = ArenaStatus.Inactive,
                SportId = 1,
                CityId = 1,
                CountryId = 1,
                ArenaAdminId = secondUserId,
                PricePerHour = 20,
            };

            this.DbContext.Arenas.Add(secondArena);
            await this.DbContext.SaveChangesAsync();

            var result = await this.Service.GetCountInCountryAsync(1);
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task AdminFilterAsyncFilterCorrectByCountryId()
        {
            var result = await this.Service.AdminFilterAsync(1, null, null);
            Assert.Null(result.CityId);
            Assert.Null(result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("testCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task AdminFilterAsyncFilterCorrectByCityId()
        {
            var result = await this.Service.AdminFilterAsync(1, 1, null);
            Assert.Equal(1, result.CityId);
            Assert.Null(result.SportId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("testCity, testCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task AdminFilterAsyncFilterCorrectBySportId()
        {
            var result = await this.Service.AdminFilterAsync(1, null, 1);
            Assert.Equal(1, result.SportId);
            Assert.Null(result.CityId);
            Assert.Single(result.Arenas);
            Assert.Equal(1, result.ResultCount);
            Assert.Equal("testCountry", result.Location);
            Assert.Single(result.Filter.Sports);
            Assert.Single(result.Filter.Cities);
        }

        [Fact]
        public async Task AdminUpdateAsyncUpdatesRecordCorectly()
        {
            var inputModel = new EditViewModel
            {
                Id = 2,
                Name = "newName",
                Address = "newAddress",
                Description = "newDescription",
                WebUrl = "newWebUrl",
                SportId = 1,
                PhoneNumber = "newPhoneNumber",
                PricePerHour = 400,
            };

            await this.Service.AdminUpdateAsync(inputModel);
            var arena = this.DbContext.Arenas.FirstOrDefault(a => a.Id == 2);

            Assert.Equal("newName", arena.Name);
            Assert.Equal("newAddress", arena.Address);
            Assert.Equal("newDescription", arena.Description);
            Assert.Equal("newWebUrl", arena.WebUrl);
            Assert.Equal(1, arena.SportId);
            Assert.Equal("newPhoneNumber", arena.PhoneNumber);
            Assert.Equal(400, arena.PricePerHour);
        }

        [Fact]
        public async Task AdminUpdateAsyncThrowsIfInvalidId()
        {
            var inputModel = new EditViewModel
            {
                Id = 22,
            };
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.AdminUpdateAsync(inputModel));
        }
    }
}
