namespace LetsSport.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Admin.Reports;
    using LetsSport.Web.ViewModels.Reports;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ReportsServiceTests : BaseServiceTests
    {
        private string senderId;
        private string recipientId;

        public ReportsServiceTests()
        {
            this.senderId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            this.recipientId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();
        }

        private IReportsService Service => this.ServiceProvider.GetRequiredService<IReportsService>();

        [Fact]
        public async Task CreateAsyncAddsReportToDb()
        {
            var viewModel = await this.Service.CreateAsync(this.recipientId, this.senderId);
            Assert.IsType<ReportInputModel>(viewModel);
            Assert.Equal(this.senderId, viewModel.SenderUserId);
            Assert.Equal(this.recipientId, viewModel.ReportedUserId);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public async Task GetAllAsyncReturnsCorrectNumber()
        {
            var reports = await this.Service.GetAllAsync<ReportInfoViewModel>();
            Assert.Single(reports);
        }

        [Fact]
        public async Task GetByIdAsyncReturnsCorrectReport()
        {
            var viewModel = await this.Service.GetByIdAsync<ReportInfoViewModel>(1);
            Assert.NotNull(viewModel);
            Assert.Equal(1, viewModel.Id);
        }

        [Fact]
        public async Task GetByIdAsyncReturnsNullWithInvalidId()
        {
            var viewModel = await this.Service.GetByIdAsync<ReportInfoViewModel>(11);
            Assert.Null(viewModel);
        }

        [Fact]
        public async Task AddAsyncShouldAddReportToDb()
        {
            await this.Service.AddAsync(this.senderId, (int)AbuseType.AggressiveLanguage, "content", this.recipientId);
            Assert.Equal(2, this.DbContext.Reports.Count());
        }

        [Fact]
        public async Task ArchiveAsyncShouldRemoveReportFromDb()
        {
            await this.Service.AddAsync(this.senderId, (int)AbuseType.AggressiveLanguage, "content", this.recipientId);
            Assert.Equal(2, this.DbContext.Reports.Count());
            await this.Service.ArchiveAsync(2);
            Assert.Equal(1, this.DbContext.Reports.Count());
        }

        [Fact]
        public async Task GetCountAsyncReturnsCorrectNumber()
        {
            Assert.Equal(1, await this.Service.GetCountAsync());
        }

        [Fact]
        public async Task FilterAsyncWorksCorrectlyWithAll()
        {
            var viewModel = await this.Service.FilterAsync(3);
            Assert.IsType<IndexViewModel>(viewModel);
            Assert.Single(viewModel.Reports);
            Assert.Equal(1, viewModel.ResultCount);
        }

        [Fact]
        public async Task FilterAsyncWorksCorrectlyWithDeleted()
        {
            await this.Service.AddAsync(this.senderId, (int)AbuseType.AggressiveLanguage, "content", this.recipientId);
            await this.Service.ArchiveAsync(2);
            var viewModel = await this.Service.FilterAsync(2);
            Assert.Single(viewModel.Reports);
            Assert.Equal(1, viewModel.ResultCount);
        }

        [Fact]
        public async Task FilterAsyncWorksCorrectlyWithNoDeleted()
        {
            var viewModel = await this.Service.FilterAsync(1);
            Assert.Single(viewModel.Reports);
            Assert.Equal(1, viewModel.ResultCount);
        }
    }
}
