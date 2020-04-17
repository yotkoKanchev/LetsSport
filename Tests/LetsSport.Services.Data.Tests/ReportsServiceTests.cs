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
            var sender = new ApplicationUser
            {
                Email = "sender@abv.bg",
                PasswordHash = "passsword",
                CityId = 1,
                CountryId = 1,
            };

            var recipient = new ApplicationUser
            {
                Email = "recipient@abv.bg",
                PasswordHash = "passsword",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(sender);
            this.DbContext.ApplicationUsers.Add(recipient);
            this.DbContext.SaveChanges();

            this.senderId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            this.recipientId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();

            var report = new Report
            {
                Abuse = AbuseType.SexualHarisement,
                Content = "content",
                SenderId = this.senderId,
                ReportedUserId = this.recipientId,
            };

            this.DbContext.Reports.Add(report);
            this.DbContext.SaveChanges();
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
    }
}
