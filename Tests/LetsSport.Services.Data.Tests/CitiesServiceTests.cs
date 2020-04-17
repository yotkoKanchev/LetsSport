namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class CitiesServiceTests : BaseServiceTests
    {
        public CitiesServiceTests()
        {
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

            var sport = new Sport
            {
                Name = "Sport",
                Image = "https://url",
            };

            this.DbContext.Sports.Add(sport);

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

            var evt = new Event
            {
                CountryId = 1,
                CityId = 1,
                Name = "Event",
                Date = new DateTime(2020, 05, 05),
                StartingHour = new DateTime(2020, 05, 05, 12, 00, 00),
                Status = EventStatus.AcceptingPlayers,
                RequestStatus = ArenaRequestStatus.NotSent,
                MinPlayers = 0,
                MaxPlayers = 1,
                Gender = Gender.Any,
                DurationInHours = 1,
                SportId = 1,
                AdminId = userId,
                ArenaId = this.DbContext.Arenas.Select(a => a.Id).First(),
            };

            this.DbContext.Events.Add(evt);
            this.DbContext.SaveChanges();
        }

        private ICitiesService Service => this.ServiceProvider.GetRequiredService<ICitiesService>();

        [Fact]
        public async Task GetIdAsyncReturnsCorrectId()
        {
            var id = await this.Service.GetIdAsync("testCity", 1);
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task GetIdAsyncThrowsIfInvalidCountryId()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.GetIdAsync("testCity", 11));
        }

        [Fact]
        public async Task GetIdAsyncThrowsIfInvalidCityName()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.GetIdAsync("city", 11));
        }

        [Fact]
        public async Task GetIdAsyncThrowsIfCityNameIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.GetIdAsync(null, 11));
        }

        [Fact]
        public async Task GetAllInCountryByIdAsyncReturnsCorrectNumberOfCities()
        {
            var cities = await this.Service.GetAllInCountryByIdAsync(1);
            foreach (var city in cities)
            {
                Assert.Equal("testCity", city.Text);
                Assert.Equal("1", city.Value);
            }
        }

        [Fact]
        public async Task GetAllWithEventsInCountryAsyncReturnsCorrectNumberOfCities()
        {
            var cities = await this.Service.GetAllWithEventsInCountryAsync(1);
            Assert.Single(cities);
        }

        [Fact]
        public async Task GetAllWithArenasInCountryAsyncReturnsCorrectNumerOfCities()
        {
            var cities = await this.Service.GetAllWithArenasInCountryAsync(1);
            Assert.Single(cities);
        }

        [Fact]
        public async Task IsExistsAsyncReturnsTrueWithValidArguments()
        {
            var result = await this.Service.IsExistsAsync("testCity", 1);
            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsAsyncReturnsFalseWithInvalidCity()
        {
            var result = await this.Service.IsExistsAsync("city", 1);
            Assert.False(result);
        }

        [Fact]
        public async Task IsExistsAsyncReturnsFalseIfCityIsNull()
        {
            var result = await this.Service.IsExistsAsync(null, 1);
            Assert.False(result);
        }

        [Fact]
        public async Task IsExistsAsyncReturnsFalseInvalidCountry()
        {
            var result = await this.Service.IsExistsAsync("city", 11);
            Assert.False(result);
        }

        [Fact]
        public async Task GetNameByIdAsyncReturnsCorrectName()
        {
            var name = await this.Service.GetNameByIdAsync(1);
            Assert.Equal("testCity", name);
        }

        [Fact]
        public async Task GetNameByIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetNameByIdAsync(11));
        }

        [Fact]
        public async Task GetAllByCountryIdAsyncReturnsCorrectNumber()
        {
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };

            this.DbContext.Cities.Add(secondCity);
            await this.DbContext.SaveChangesAsync();

            var cities = await this.Service.GetAllByCountryIdAsync<CityInfoViewModel>(1);
            Assert.Equal(2, cities.Count());

            var noCities = await this.Service.GetAllByCountryIdAsync<CityInfoViewModel>(11);
            Assert.Empty(noCities);
        }

        [Fact]
        public async Task FilterAsyncFilterByCountry()
        {
            var viewModel = await this.Service.FilterAsync(1, 3);
            Assert.Single(viewModel.Cities);
            Assert.IsType<IndexViewModel>(viewModel);
        }

        [Fact]
        public async Task FilterAsyncFilterWithDeleted()
        {
            var viewModel = await this.Service.FilterAsync(1, 2);
            Assert.Empty(viewModel.Cities);
            Assert.IsType<IndexViewModel>(viewModel);
        }

        // [Fact]
        // public async Task FilterAsyncFilterWithNoDeleted()
        // {
        //    var viewModel = await this.Service.FilterAsync(1, 1);
        //    Assert.Empty(viewModel.Cities);
        //    Assert.IsType<IndexViewModel>(viewModel);
        // }
        [Fact]
        public async Task FilterAsyncFilterReturnsZeroCitiesWithEmptyCountry()
        {
            var country = new Country
            {
                Name = "secondCountry",
            };

            this.DbContext.Countries.Add(country);
            await this.DbContext.SaveChangesAsync();
            var viewModel = await this.Service.FilterAsync(2, 3);
            Assert.Empty(viewModel.Cities);
            Assert.IsType<IndexViewModel>(viewModel);
        }

        [Fact]
        public async Task FilterAsyncThrowsWithInvalidCountryId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.FilterAsync(11, 3));
        }

        [Fact]
        public async Task GetByIdAsyncReturnsCorrectModel()
        {
            var model = await this.Service.GetByIdAsync<CityInfoViewModel>(1);
            Assert.Equal("testCity", model.Name);
            Assert.Equal(1, model.CountryId);
        }

        [Fact]
        public async Task GetByIdAsyncWithInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetByIdAsync<CityInfoViewModel>(11));
        }

        [Fact]
        public async Task CreateAsyncAddCityToDb()
        {
            await this.Service.CreateAsync("secondCity", 1);
            Assert.Equal(2, this.DbContext.Cities.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsIfCityExists()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.CreateAsync("testCity", 1));
        }

        [Fact]
        public async Task CreateAsyncThrowsIfInvalidCountryId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.CreateAsync("secondCity", 11));
        }

        [Fact]
        public async Task UpdateAsyncUpdatesCorrectly()
        {
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };

            this.DbContext.Cities.Add(secondCity);
            await this.DbContext.SaveChangesAsync();

            var newName = "updateName";

            await this.Service.UpdateAsync(2, newName, 1, false);
            var city = await this.Service.GetByIdAsync<CityInfoViewModel>(2);
            Assert.Equal("updateName", city.Name);
        }

        [Fact]
        public async Task UpdateAsyncThrowsIfNameExists()
        {
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };

            this.DbContext.Cities.Add(secondCity);
            await this.DbContext.SaveChangesAsync();
            var newName = "testCity";

            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(1, newName, 1, false));
        }

        [Fact]
        public async Task ArchiveByIdAsyncArchivesCityCorrectly()
        {
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };

            this.DbContext.Cities.Add(secondCity);
            await this.DbContext.SaveChangesAsync();

            await this.Service.ArchiveByIdAsync(2);
            Assert.Equal(1, await this.Service.GetCountInCountryAsync(1));
            var city = await this.Service.GetByIdAsync<CityInfoViewModel>(2);
            Assert.True(city.IsDeleted);
        }

        [Fact]
        public async Task ArchiveByIdThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(11, "name", 1, false));
        }

        [Fact]
        public async Task DeleteByIdAsyncArchivesCityCorrectly()
        {
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };

            this.DbContext.Cities.Add(secondCity);
            await this.DbContext.SaveChangesAsync();
            await this.Service.DeleteByIdAsync(2);
            Assert.Equal(1, this.DbContext.Cities.Count());
        }

        [Fact]
        public async Task DeleeeByIdThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.DeleteByIdAsync(11));
        }

        [Fact]
        public async Task GetCountInCountryAsyncReturnsCorrectNumber()
        {
            var count = await this.Service.GetCountInCountryAsync(1);
            Assert.Equal(1, count);
        }
    }
}
