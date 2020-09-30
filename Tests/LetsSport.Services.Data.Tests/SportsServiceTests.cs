namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Services.Data.Sports;
    using LetsSport.Web.ViewModels.Admin.Sports;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class SportsServiceTests : BaseServiceTests
    {
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
        public async Task DeleteByIdAsyncRemoveFromDb()
        {
            await this.Service.CreateAsync("secondSport", "https://secondSportUrl");
            await this.Service.DeleteByIdAsync(2);
            Assert.Equal(1, this.DbContext.Sports.Count());
        }

        [Fact]
        public async Task GetCountAsyncReturnCorrectNumber()
        {
            var count = await this.Service.GetCountAsync();
            Assert.Equal(1, count);
        }
    }
}
