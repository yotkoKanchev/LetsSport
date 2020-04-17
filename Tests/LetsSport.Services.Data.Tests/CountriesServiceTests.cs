namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Admin.Countries;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class CountriesServiceTests : BaseServiceTests
    {
        public CountriesServiceTests()
        {
            var country = new Country
            {
                Name = "testCountry",
            };

            this.DbContext.Countries.Add(country);
            this.DbContext.SaveChanges();
        }

        private ICountriesService Service => this.ServiceProvider.GetRequiredService<ICountriesService>();

        [Fact]
        public async Task CreateAsyncShouldCreateCountryCorrectly()
        {
            string name = "secondCountry";
            await this.Service.CreateAsync(name);
            var country = this.DbContext.Countries.Skip(1).FirstOrDefault();

            Assert.Equal(name, country.Name);
            Assert.Equal(2, country.Id);
            Assert.Equal(2, this.DbContext.Countries.Count());
        }

        [Fact]
        public async Task CreateAsynThrowsIfExists()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.CreateAsync("testCountry"));
        }

        [Fact]
        public async Task GetAllAsSelectListAsyncShouldReturnCorrectNumber()
        {
            var countries = await this.Service.GetAllAsSelectListAsync();
            foreach (var country in countries)
            {
                Assert.Equal("testCountry", country.Text);
                Assert.Equal("1", country.Value);
            }
        }

        [Fact]
        public async Task GetIdAsyncReturnsCorrectId()
        {
            var id = await this.Service.GetIdAsync("testCountry");
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task GetNameByIdAsyncReturnCorrectName()
        {
            var name = await this.Service.GetNameByIdAsync(1);
            Assert.Equal("testCountry", name);
        }

        [Fact]
        public async Task GetNameByIdAsyncThrowsErrorIfInvalidIdGiven()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.GetNameByIdAsync(11));
        }

        [Fact]
        public async Task GetByIdAsyncShouldReturnCorrectModel()
        {
            var country = await this.Service.GetByIdAsync<CountryInfoViewModel>(1);
            Assert.Equal(1, country.Id);
            Assert.Equal("testCountry", country.Name);
        }

        [Fact]
        public async Task GetByIdAsyncThrowsErrorIfInvalidIdGiven()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetByIdAsync<CountryInfoViewModel>(11));
        }

        [Fact]
        public async Task GetAllAsyncReturnsCorrectNumberOfCountries()
        {
            string name = "secondCountry";
            await this.Service.CreateAsync(name);

            var viewModels = await this.Service.GetAllAsync<CountryInfoViewModel>();
            Assert.Equal(2, viewModels.Count());
        }

        [Fact]
        public async Task GetCountAsyncReturnsCorrectNumber()
        {
            string name = "secondCountry";
            await this.Service.CreateAsync(name);

            var count = await this.Service.GetCountAsync();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task UpdateAsyncUpdatesCorrectly()
        {
            string name = "secondCountry";
            await this.Service.CreateAsync(name);
            await this.Service.UpdateAsync(2, "newName");

            var country = await this.Service.GetByIdAsync<CountryInfoViewModel>(2);
            Assert.Equal("newName", country.Name);
        }

        [Fact]
        public async Task UpdateAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(11, "newName"));
        }

        [Fact]
        public async Task UpdateAsyncThrowsIfNameExists()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(1, "testCountry"));
        }

        [Fact]
        public async Task UpdateAsyncThrowsIfNameIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.UpdateAsync(1, null));
        }

        [Fact]
        public async Task DeleteByIdAsyncArchiveCountry()
        {
            string name = "secondCountry";
            await this.Service.CreateAsync(name);
            Assert.Equal(2, await this.Service.GetCountAsync());
            await this.Service.DeleteByIdAsync(2);
            Assert.Equal(1, await this.Service.GetCountAsync());
        }

        [Fact]
        public async Task DeleteByIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
              => this.Service.DeleteByIdAsync(11));
        }

        [Fact]
        public async Task IsValidIdReturnsTrueWithValidId()
        {
            Assert.True(await this.Service.IsValidId(1));
        }

        [Fact]
        public async Task IsValidIdReturnsFalseWithInvalidId()
        {
            Assert.False(await this.Service.IsValidId(11));
        }
    }
}
