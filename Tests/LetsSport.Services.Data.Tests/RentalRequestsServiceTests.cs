namespace LetsSport.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class RentalRequestsServiceTests : BaseServiceTests
    {
        public RentalRequestsServiceTests()
        {
            this.Dispose();
        }

        private IRentalRequestsService Service => this.ServiceProvider.GetRequiredService<IRentalRequestsService>();

        [Fact]
        public async Task CreateAsyncShouldAddRequestToDb()
        {
            Assert.Equal(0, this.DbContext.ArenaRentalRequests.Count());
            await this.Service.CreateAsync(1, 1);
            Assert.Equal(1, this.DbContext.ArenaRentalRequests.Count());
            var request = this.DbContext.ArenaRentalRequests.First();
            Assert.Equal(ArenaRentalRequestStatus.NotApproved, request.Status);
        }

        [Fact]
        public async Task ChangeStatusAsyncSetApprovedStatus()
        {
            await this.Service.CreateAsync(1, 1);
            var requestId = this.DbContext.ArenaRentalRequests.Select(ar => ar.Id).First();
            await this.Service.ChangeStatusAsync(requestId, (ArenaRentalRequestStatus)2);
            var request = this.DbContext.ArenaRentalRequests.First();
            Assert.Equal(ArenaRentalRequestStatus.Approved, request.Status);
        }

        [Fact]
        public async Task ChangeStatusAsyncSetDeniedStatus()
        {
            await this.Service.CreateAsync(1, 1);
            var requestId = this.DbContext.ArenaRentalRequests.Select(ar => ar.Id).First();
            await this.Service.ChangeStatusAsync(requestId, (ArenaRentalRequestStatus)3);
            var request = this.DbContext.ArenaRentalRequests.First();
            Assert.Equal(ArenaRentalRequestStatus.Denied, request.Status);
        }

        [Fact]
        public async Task ChangeStatusAsyncSetPassedStatus()
        {
            await this.Service.CreateAsync(1, 1);
            var requestId = this.DbContext.ArenaRentalRequests.Select(ar => ar.Id).First();
            await this.Service.ChangeStatusAsync(requestId, (ArenaRentalRequestStatus)4);
            var request = this.DbContext.ArenaRentalRequests.First();
            Assert.Equal(ArenaRentalRequestStatus.Passed, request.Status);
        }

        [Fact]
        public async Task ChangeStatusAsyncSetCanceledStatus()
        {
            await this.Service.CreateAsync(1, 1);
            var requestId = this.DbContext.ArenaRentalRequests.Select(ar => ar.Id).First();
            await this.Service.ChangeStatusAsync(requestId, (ArenaRentalRequestStatus)5);
            var request = this.DbContext.ArenaRentalRequests.First();
            Assert.Equal(ArenaRentalRequestStatus.Canceled, request.Status);
        }
    }
}
