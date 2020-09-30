namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Messages;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class MessagesServiceTests : BaseServiceTests
    {
        private const string NoAvatarPath = "../../images/noAvatar.png";
        private string userId;
        private string messageId;

        public MessagesServiceTests()
        {
            this.userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            this.messageId = this.DbContext.Messages.Select(m => m.Id).First();
        }

        private IMessagesService Service => this.ServiceProvider.GetRequiredService<IMessagesService>();

        [Fact]
        public async Task CreateAsyncShouldAddMessageInDb()
        {
            Assert.Equal(1, this.DbContext.Messages.Count());
            await this.Service.CreateAsync("content", this.userId, 1);
            Assert.Equal(2, this.DbContext.Messages.Count());
        }

        [Fact]
        public async Task GetAllByEventIdAsyncShouldWorksCorectly()
        {
            var messages = await this.Service.GetAllByEventIdAsync(1);
            Assert.Single(messages);
            var newMessage = new Message
            {
                Content = "test",
                EventId = 1,
                SenderId = this.userId,
            };

            this.DbContext.Messages.Add(newMessage);
            await this.DbContext.SaveChangesAsync();
            messages = await this.Service.GetAllByEventIdAsync(1);
            Assert.Equal(2, messages.Count());
        }

        [Fact]
        public async Task GetDetailsByIdReturnsCorrectDetails()
        {
            var viewModel = await this.Service.GetDetailsById(this.messageId);
            Assert.Equal("testMessage", viewModel.Content);
            Assert.Equal(this.userId, viewModel.SenderId);
            Assert.Equal(NoAvatarPath, viewModel.SenderAvatarUrl);
        }

        [Fact]
        public async Task GetDetailsByIdThrowsWithInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsById("someId"));
        }
    }
}
