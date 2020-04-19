namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Web.ViewModels.Admin.Events;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class EventsServiceTests : BaseServiceTests
    {
        private readonly string userId;
        private readonly string secondUserId;

        public EventsServiceTests()
        {
            this.userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            var secondCity = new City
            {
                Name = "secondCity",
                CountryId = 1,
            };
            this.DbContext.Cities.Add(secondCity);

            var secondSport = new Sport
            {
                Name = "secondSport",
                Image = "https://newSportImage",
            };

            this.DbContext.Sports.Add(secondSport);
            this.DbContext.SaveChanges();

            var secondUser = new ApplicationUser
            {
                Email = "secondEmail",
                PasswordHash = "sssdddfff",
                CityId = 2,
                CountryId = 1,
                SportId = 2,
            };

            this.DbContext.ApplicationUsers.Add(secondUser);
            this.DbContext.SaveChanges();

            this.secondUserId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();

            var secondArena = new Arena
            {
                Name = "secondArena",
                CountryId = 1,
                CityId = 2,
                SportId = 2,
                Status = ArenaStatus.Active,
                ArenaAdminId = this.secondUserId,
                PricePerHour = 200,
                PhoneNumber = "0898998822",
                Email = "secondArenaEmail@abv.bg",
                WebUrl = "https://webUrl",
            };

            this.DbContext.Arenas.Add(secondArena);

            var secondEvent = new Event
            {
                CountryId = 1,
                CityId = 1,
                Name = "secondEvent",
                Date = DateTime.Now.AddMonths(3),
                StartingHour = DateTime.Now.AddMonths(3),
                Status = EventStatus.AcceptingPlayers,
                RequestStatus = ArenaRequestStatus.NotSent,
                MinPlayers = 1,
                MaxPlayers = 10,
                Gender = Gender.Any,
                DurationInHours = 1,
                SportId = 1,
                AdminId = this.secondUserId,
                ArenaId = this.DbContext.Arenas.Select(a => a.Id).First(),
            };

            var canceledEvent = new Event
            {
                CountryId = 1,
                CityId = 1,
                Name = "canceledEvent",
                Date = DateTime.Now.AddMonths(1),
                StartingHour = DateTime.Now.AddMonths(1),
                Status = EventStatus.Canceled,
                RequestStatus = ArenaRequestStatus.NotSent,
                MinPlayers = 1,
                MaxPlayers = 10,
                Gender = Gender.Any,
                DurationInHours = 1,
                SportId = 1,
                AdminId = this.userId,
                ArenaId = this.DbContext.Arenas.Select(a => a.Id).First(),
            };

            this.DbContext.Events.Add(secondEvent);
            this.DbContext.Events.Add(canceledEvent);
            this.DbContext.SaveChanges();
        }

        private IEventsService Service => this.ServiceProvider.GetRequiredService<IEventsService>();

        [Fact]
        public async Task GetAllInCityAsyncShouldReturnCorrectResult()
        {
            var result = await this.Service.GetAllInCityAsync<EventCardPartialViewModel>(1, 1);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCountInCityAsyncShouldReturnCorrectCount()
        {
            var result = await this.Service.GetCountInCityAsync(1);
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetAllAdministratingByUserIdAsyncShouldReturnCorrectCount()
        {
            var result = await this.Service
                .GetAllAdministratingByUserIdAsync<EventCardPartialViewModel>(1, this.userId);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllUpcomingByUserIdAsyncReturnsCorrectCount()
        {
            var result = await this.Service
                            .GetAllUpcomingByUserIdAsync<EventCardPartialViewModel>(1, this.userId);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetNotParticipatingInCityAsyncReturnsCorrectCount()
        {
            var result = await this.Service
                            .GetNotParticipatingInCityAsync<EventCardPartialViewModel>(1, this.userId, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetNotParticipatingCountReturnsCorrectCount()
        {
            var result = await this.Service.GetNotParticipatingCount(this.userId, 1);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetAdminAllCanceledAsyncReturnsCorrectCount()
        {
            var result = await this.Service
                .GetAdminAllCanceledAsync<EventCardPartialViewModel>(this.userId);

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsyncAddEventToDb()
        {
            var inputModel = new EventCreateInputModel
            {
                AdminId = this.userId,
                ArenaId = 1,
                SportId = 1,
                Date = DateTime.UtcNow.AddMonths(3),
                StartingHour = DateTime.UtcNow.AddMonths(3),
                MinPlayers = 4,
                MaxPlayers = 10,
                DurationInHours = 2,
                Name = "ThirdEvent",
                Gender = Gender.Any,
                RequestStatus = ArenaRequestStatus.NotSent,
            };

            await this.Service.CreateAsync(inputModel, 1, 1, this.userId, "email", "username");

            Assert.Equal(4, this.DbContext.Events.Count());
            Assert.Equal(2, this.DbContext.EventsUsers.Count());
            Assert.Equal(2, this.DbContext.Messages.Count());
        }

        [Fact]
        public async Task GetDetailsAsyncReturnsCorrectDetails()
        {
            var result = await this.Service.GetDetailsAsync(1, this.userId);
            Assert.Equal(this.userId, result.AdminId);
            Assert.Equal("tester", result.AdminUserName);
            Assert.Equal(1, result.ArenaId);
            Assert.Equal("testArena", result.ArenaName);
            Assert.Equal("testSport", result.SportName);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, result.Date.Date);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, result.StartingHour.Date);
            Assert.Equal(1, result.DurationInHours);
            Assert.Equal("Accepting Players", result.Status);
            Assert.Equal(1, result.MinPlayers);
            Assert.Equal(10, result.MaxPlayers);
            Assert.Equal(Gender.Any, result.Gender);
        }

        [Fact]
        public async Task GetDetailsAsyncThrowsIfInvalidEventId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetDetailsAsync(11, this.userId));
        }

        [Fact]
        public async Task GetDetailsForEditAsyncReturnsCorrectDetails()
        {
            var result = await this.Service.GetDetailsForEditAsync(1);
            Assert.Equal(this.userId, result.AdminId);
            Assert.Equal(1, result.SportId);
            Assert.Equal(1, result.ArenaId);
            Assert.Equal(1, result.CityId);
            Assert.Equal(1, result.ArenaId);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, result.Date.Date);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, result.StartingHour.Date);
            Assert.Equal(1, result.DurationInHours);
            Assert.Equal(1, result.MinPlayers);
            Assert.Equal(10, result.MaxPlayers);
            Assert.Equal(Gender.Any, result.Gender);
        }

        [Fact]
        public async Task GetDetailsForEditAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.GetDetailsForEditAsync(11));
        }

        [Fact]
        public async Task UpdateAsyncUpdatesRecordInDb()
        {
            var inputModel = new EventEditViewModel()
            {
                Id = 2,
                CityId = 1,
                SportId = 1,
                Date = DateTime.UtcNow.AddMonths(1),
                StartingHour = DateTime.UtcNow.AddMonths(1),
                AdditionalInfo = "info",
                DurationInHours = 5,
                Gender = Gender.Male,
                MinPlayers = 6,
                MaxPlayers = 12,
                Name = "newName",
                GameFormat = "gameFormat",
            };

            await this.Service.UpdateAsync(inputModel);

            var evt = this.DbContext.Events.First(e => e.Id == 2);

            Assert.Equal(1, evt.CityId);
            Assert.Equal(1, evt.SportId);
            Assert.Equal(DateTime.UtcNow.AddMonths(1).Date, evt.Date.Date);
            Assert.Equal(DateTime.UtcNow.AddMonths(1).Date, evt.StartingHour.Date);
            Assert.Equal("info", evt.AdditionalInfo);
            Assert.Equal(5, evt.DurationInHours);
            Assert.Equal(Gender.Male, evt.Gender);
            Assert.Equal(6, evt.MinPlayers);
            Assert.Equal(12, evt.MaxPlayers);
            Assert.Equal("newName", evt.Name);
            Assert.Equal("gameFormat", evt.GameFormat);
        }

        [Fact]
        public async Task UpdateAsyncThrowsIfInvalidId()
        {
            var inputModel = new EventEditViewModel() { Id = 11, };
            await Assert.ThrowsAsync<ArgumentException>(() => this.Service.UpdateAsync(inputModel));
        }

        [Fact]
        public async Task CancelEventAsyncRemovesEventAndEventUserFromDb()
        {
            await this.Service.CancelEventAsync(3, "email", "username");
            Assert.Equal(3, this.DbContext.Events.Count());
            Assert.Equal(1, this.DbContext.EventsUsers.Count());
        }

        [Fact]
        public async Task CancelEventAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.CancelEventAsync(11, "email", "username"));
        }

        [Fact]
        public async Task AddUserAsyncAddsUserToEvent()
        {
            var newUser = new ApplicationUser
            {
                Email = "newUserEmail@abv.bg",
                PasswordHash = "hash",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(newUser);
            await this.DbContext.SaveChangesAsync();
            var newUserId = this.DbContext.ApplicationUsers
                .Where(u => u.Email == "newUserEmail@abv.bg").Select(u => u.Id).First();
            await this.Service.AddUserAsync(1, newUserId, "email", "username");

            var eventUser = this.DbContext.EventsUsers
                .FirstOrDefault(eu => eu.UserId == newUserId && eu.EventId == 1);

            Assert.NotNull(eventUser);
            Assert.Equal(2, this.DbContext.Messages.Count());
            Assert.Equal(2, this.DbContext.EventsUsers.Count());
        }

        [Fact]
        public async Task InviteUsersToEventAsyncReturnsCorrectInvitedUsersCount()
        {
            var newUser = new ApplicationUser
            {
                Email = "newUserEmail@abv.bg",
                PasswordHash = "hash",
                CityId = 1,
                CountryId = 1,
                SportId = 1,
                Status = UserStatus.ProposalOpen,
            };

            this.DbContext.ApplicationUsers.Add(newUser);
            await this.DbContext.SaveChangesAsync();
            var result = await this.Service.InviteUsersToEventAsync(1, "email", "username");
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task InviteUsersToEventAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.InviteUsersToEventAsync(11, "email", "username"));
        }

        [Fact]
        public async Task RemoveUserAsyncRemovesUserFromEvent()
        {
            var newUser = new ApplicationUser
            {
                Email = "newUserEmail@abv.bg",
                PasswordHash = "hash",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(newUser);
            await this.DbContext.SaveChangesAsync();
            var newUserId = this.DbContext.ApplicationUsers
                .Where(u => u.Email == "newUserEmail@abv.bg").Select(u => u.Id).First();
            await this.Service.AddUserAsync(1, newUserId, "email", "username");
            Assert.Equal(2, this.DbContext.EventsUsers.Count());

            await this.Service.RemoveUserAsync(1, newUserId, "email", "username");
            Assert.Equal(1, this.DbContext.EventsUsers.Count());
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithNoFilter()
        {
            var result = await this.Service
                .FilterAsync(null, null, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6), 1, null);
            Assert.Equal(2, result.Events.Count());
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithCityId()
        {
            var thirdEvent = new Event
            {
                CountryId = 1,
                CityId = 2,
                SportId = 2,
                Name = "thirdEvent",
                Date = DateTime.Now.AddMonths(4),
                StartingHour = DateTime.Now.AddMonths(4),
                Status = EventStatus.AcceptingPlayers,
                RequestStatus = ArenaRequestStatus.NotSent,
                MinPlayers = 1,
                MaxPlayers = 10,
                Gender = Gender.Any,
                DurationInHours = 1,
                AdminId = this.secondUserId,
                ArenaId = this.DbContext.Arenas.Where(a => a.Email == "secondArenaEmail@abv.bg").Select(a => a.Id).First(),
            };

            this.DbContext.Events.Add(thirdEvent);
            this.DbContext.SaveChanges();

            var result = await this.Service
               .FilterAsync(2, null, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6), 1, null);
            Assert.Single(result.Events);
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithDates()
        {
            var result = await this.Service
                           .FilterAsync(1, null, DateTime.UtcNow.AddDays(60), DateTime.UtcNow.AddMonths(6), 1, null);
            Assert.Equal(2, result.Events.Count());
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithSport()
        {
            var result = await this.Service
                           .FilterAsync(null, 1, DateTime.UtcNow.AddMonths(1), DateTime.UtcNow.AddMonths(6), 1, null);
            Assert.Equal(2, result.Events.Count());
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithSportWhitPaging()
        {
            var result = await this.Service
                           .FilterAsync(null, 1, DateTime.UtcNow.AddMonths(1), DateTime.UtcNow.AddMonths(6), 1, null, 1);
            Assert.Single(result.Events);
        }

        [Fact]
        public async Task FilterEventsAsyncReturnCorrectResultWithUserId()
        {
            var result = await this.Service
                .FilterAsync(null, null, DateTime.UtcNow, DateTime.UtcNow.AddMonths(6), 1, this.userId);
            Assert.Single(result.Events);
        }

        [Fact]
        public async Task ChangeStatusSetStatusCorrectly()
        {
            var evt = this.DbContext.Events.FirstOrDefault(e => e.Id == 1);
            Assert.Equal(ArenaRequestStatus.NotSent, evt.RequestStatus);

            await this.Service.ChangeStatus(1, ArenaRequestStatus.Sent);
            Assert.Equal(ArenaRequestStatus.Sent, evt.RequestStatus);

            await this.Service.ChangeStatus(1, ArenaRequestStatus.Denied);
            Assert.Equal(ArenaRequestStatus.Denied, evt.RequestStatus);

            await this.Service.ChangeStatus(1, ArenaRequestStatus.Approved);
            Assert.Equal(ArenaRequestStatus.Approved, evt.RequestStatus);

            await this.Service.ChangeStatus(1, ArenaRequestStatus.NotSent);
            Assert.Equal(ArenaRequestStatus.NotSent, evt.RequestStatus);
        }

        [Fact]
        public void IsUserJoinedReturnsFalseWithInvalidArguments()
        {
            Assert.True(this.Service.IsUserJoined(this.userId, 1));
        }

        [Fact]
        public async Task IsUserJoinedReturnsTrueWithValidArguments()
        {
            await this.Service.AddUserAsync(1, this.secondUserId, "email", "username");
            Assert.True(this.Service.IsUserJoined(this.secondUserId, 1));
        }

        [Fact]
        public async Task IsUserAdminOnEventAsyncReturnsCorrectResult()
        {
            Assert.True(await this.Service.IsUserAdminOnEventAsync(this.userId, 1));
            Assert.False(await this.Service.IsUserAdminOnEventAsync(this.secondUserId, 1));
        }

        [Fact]
        public async Task GetEventByRequestIdAsyncReturnCorrectEvent()
        {
            var requestId = this.DbContext.ArenaRentalRequests.Select(rr => rr.Id).FirstOrDefault();
            var evt = await this.Service.GetEventByRequestIdAsync(requestId);

            Assert.Equal(1, evt.Id);
        }

        [Fact]
        public async Task GetEventByRequestIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.GetEventByRequestIdAsync("id"));
        }

        [Fact]
        public async Task GetAllInCountryAsyncReturnsCorrectNumber()
        {
            var result = await this.Service.GetAllInCountryAsync<InfoViewModel>(1);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllInCountry()
        {
            var result = await this.Service.AdminFilterAsync(1, null, null);
            Assert.Equal(3, result.Events.Count());
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllInCityWithEvent()
        {
            var result = await this.Service.AdminFilterAsync(1, 1, null);
            Assert.Equal(3, result.Events.Count());
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllInCityWithNoEvent()
        {
            var result = await this.Service.AdminFilterAsync(1, 2, null);
            Assert.Empty(result.Events);
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllBySportIdWithEvents()
        {
            var result = await this.Service.AdminFilterAsync(1, null, 1);
            Assert.Equal(3, result.Events.Count());
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllBySportIdWithNoEvents()
        {
            var result = await this.Service.AdminFilterAsync(1, null, 2);
            Assert.Empty(result.Events);
        }

        [Fact]
        public async Task AdminFilterAsyncReturnsAllByCityAndSport()
        {
            var result = await this.Service.AdminFilterAsync(1, 1, 1);
            Assert.Equal(3, result.Events.Count());
        }

        [Fact]
        public async Task GetEventByIdAsyncReturnsCorrectEvent()
        {
            var model = await this.Service.GetEventByIdAsync<DetailsViewModel>(1);
            Assert.Equal("Event", model.Name);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, model.Date.Date);
            Assert.Equal(DateTime.UtcNow.AddMonths(2).Date, model.StartingHour.Date);
            Assert.Equal(1, model.DurationInHours);
            Assert.Equal(1, model.MinPlayers);
            Assert.Equal(10, model.MaxPlayers);
            Assert.Equal(Gender.Any, model.Gender);
            Assert.Equal(EventStatus.AcceptingPlayers, model.Status);
        }

        [Fact]
        public async Task AdminUpdateAsyncUpdatesEventCorrectly()
        {
            var model = new EditViewModel
            {
                Id = 1,
                Name = "newName",
                DurationInHours = 2,
                MinPlayers = 10,
                MaxPlayers = 20,
                GameFormat = "newGameFormat",
                Gender = Gender.Female,
                Status = EventStatus.MinimumPlayersReached,
                RequestStatus = ArenaRequestStatus.Sent,
            };

            await this.Service.AdminUpdateAsync(model);

            var evt = this.DbContext.Events.FirstOrDefault(e => e.Id == 1);
            Assert.Equal("newName", evt.Name);
            Assert.Equal("newGameFormat", evt.GameFormat);
            Assert.Equal(2, evt.DurationInHours);
            Assert.Equal(10, evt.MinPlayers);
            Assert.Equal(20, evt.MaxPlayers);
            Assert.Equal(Gender.Female, evt.Gender);
            Assert.Equal(EventStatus.MinimumPlayersReached, evt.Status);
            Assert.Equal(ArenaRequestStatus.Sent, evt.RequestStatus);
        }

        [Fact]
        public async Task AdminUpdateAsyncThrowsIfInvalidId()
        {
            var model = new EditViewModel
            {
                Id = 11,
            };

            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.AdminUpdateAsync(model));
        }

        [Fact]
        public async Task GetCountInCountryAsyncReturnsCorrectCount()
        {
            var result = await this.Service.GetCountInCountryAsync(1);
            Assert.Equal(3, result);
        }
    }
}
