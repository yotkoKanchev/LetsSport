namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.Admin.Sports;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class SportsServiceTests : BaseServiceTests
    {
        public SportsServiceTests()
        {
            var sport = new Sport
            {
                Name = "testSport",
                Image = "https://sportUrl",
            };

            this.DbContext.Sports.Add(sport);

            var country = new Country
            {
                Name = "testCountry",
            };

            this.DbContext.Countries.Add(country);

            var city = new City
            {
                CountryId = 1,
                Name = "testCity",
                IsDeleted = false,
            };
            this.DbContext.Cities.Add(city);
            var user = new ApplicationUser
            {
                Email = "user@abv.bg",
                PasswordHash = "passsword",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(user);

            this.DbContext.SaveChanges();
            var userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            var arena = new Arena
            {
                Name = "Arena",
                SportId = 1,
                ArenaAdminId = userId,
                CityId = 1,
                CountryId = 1,
                PricePerHour = 20,
                PhoneNumber = "0888899898",
                Status = ArenaStatus.Active,
            };

            this.DbContext.Arenas.Add(arena);
            this.DbContext.SaveChanges();
        }

        private ISportsService Service => this.ServiceProvider.GetRequiredService<ISportsService>();

        [Fact]
        public async Task GetAllAsSelectListAsyncReturnsCorrectResult()
        {
            var result = await this.Service.GetAllAsSelectListAsync();
            foreach (var sport in result)
            {
                Assert.Equal("testSport", sport.Text);
                Assert.Equal("1", sport.Value);
            }
        }

        [Fact]
        public async Task GetAllInCountryByIdAsyncReturnsCorrectResult()
        {
            var result = await this.Service.GetAllInCountryByIdAsync(1);
            foreach (var sport in result)
            {
                Assert.Equal("testSport", sport.Text);
                Assert.Equal("1", sport.Value);
            }
        }

        [Fact]
        public async Task GetAllInCityByIdAsyncReturnsCorrectResult()
        {
            var result = await this.Service.GetAllInCityByIdAsync(1);
            foreach (var sport in result)
            {
                Assert.Equal("testSport", sport.Text);
                Assert.Equal("1", sport.Value);
            }
        }

        [Fact]
        public async Task GetImageByNameAsyncReturnsCorrectUrl()
        {
            var url = await this.Service.GetImageByNameAsync("testSport");
            Assert.Equal("https://sportUrl", url);
        }

        [Fact]
        public async Task GetImageByNameAsyncReturnsNullWithInvalidSport()
        {
            var url = await this.Service.GetImageByNameAsync("sport");
            Assert.Null(url);
        }

        [Fact]
        public async Task GetNameByIdAsyncReturnsCorrectName()
        {
            var name = await this.Service.GetNameByIdAsync(1);
            Assert.Equal("testSport", name);
        }

        [Fact]
        public async Task GetNameByIdAsyncThrowsWithInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetNameByIdAsync(11));
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllActiveCities()
        {
            var cities = await this.Service.GetAllAsync<SportInfoViewModel>();
            Assert.Single(cities);
        }

        [Fact]
        public async Task GetByIdAsyncReturnsCorrectSport()
        {
            var sport = await this.Service.GetByIdAsync<SportInfoViewModel>(1);
            Assert.Equal("testSport", sport.Name);
            Assert.Equal("https://sportUrl", sport.Image);
        }

        [Fact]
        public async Task GetByIdAsyncThrowsWithInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetByIdAsync<SportInfoViewModel>(11));
        }

        [Fact]
        public async Task CreateAsyncAddSportInDb()
        {
            await this.Service.CreateAsync("secondSport", "https://secondSportUrl");
            Assert.Equal(2, this.DbContext.Sports.Count());
        }

        [Fact]
        public async Task UpdateAsyncUpdateCorrectly()
        {
            await this.Service.CreateAsync("secondSport", "https://secondSportUrl");
            var newName = "newName";
            await this.Service.UpdateAsync(2, newName, null);
            var sportName = await this.Service.GetNameByIdAsync(2);
            Assert.Equal(newName, sportName);
        }

        [Fact]
        public async Task UpdateAsyncUpdateThrowsIfNameExists()
        {
            await this.Service.CreateAsync("secondSport", "https://secondSportUrl");
            var newName = "testSport";
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(2, newName, null));
        }

        [Fact]
        public async Task DeleteByIdAsyncRemoveFromDb()
        {
            await this.Service.CreateAsync("secondSport", "https://secondSportUrl");
            await this.Service.DeleteByIdAsync(2);
            Assert.Equal(1, this.DbContext.Sports.Count());
        }

        [Fact]
        public async Task DeleteByIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.DeleteByIdAsync(11));
        }

        [Fact]
        public async Task GetCountAsyncReturnCorrectNumber()
        {
            var count = await this.Service.GetCountAsync();
            Assert.Equal(1, count);
        }
    }
}
