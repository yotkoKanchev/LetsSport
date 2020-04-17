namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class MessagesServiceTests : BaseServiceTests
    {
        private string userId;

        public MessagesServiceTests()
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

            this.userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            var arena = new Arena
            {
                Name = "Arena",
                SportId = 1,
                ArenaAdminId = this.userId,
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
                AdminId = this.userId,
                ArenaId = this.DbContext.Arenas.Select(a => a.Id).First(),
            };

            this.DbContext.Events.Add(evt);
            this.DbContext.SaveChanges();

            var message = new Message
            {
                SenderId = this.userId,
                EventId = 1,
                Content = "testMessage",
            };

            this.DbContext.Messages.Add(message);
            this.DbContext.SaveChanges();
        }

        private IMessagesService Service => this.ServiceProvider.GetRequiredService<IMessagesService>();

        [Fact]
        public async Task CreateAsyncShouldAddMessageInDb()
        {
            Assert.Equal(1, this.DbContext.Messages.Count());
            await this.Service.CreateAsync("content", this.userId, 1);
            Assert.Equal(2, this.DbContext.Messages.Count());
        }
    }
}
