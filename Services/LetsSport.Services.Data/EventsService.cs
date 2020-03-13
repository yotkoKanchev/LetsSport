namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Messages;

    public class EventsService : IEventsService
    {
        private readonly IArenasService arenasService;
        private readonly ISportsService sportsService;
        private readonly IMessagesService messagesService;
        private readonly IUsersProfileService usersProfileService;
        private readonly IRepository<Event> eventsRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;

        public EventsService(
            IArenasService arenasService,
            ISportsService sportsService,
            IMessagesService messagesService,
            IUsersProfileService usersProfileService,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository)
        {
            this.arenasService = arenasService;
            this.sportsService = sportsService;
            this.messagesService = messagesService;
            this.usersProfileService = usersProfileService;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId)
        {
            var @event = inputModel.To<EventCreateInputModel, Event>(); // ASK NIKI here !!!
            //var @event = new Event
            //{
            //    Name = inputModel.Name,
            //    SportId = inputModel.SportId,
            //    MinPlayers = inputModel.MinPlayers,
            //    MaxPlayers = inputModel.MaxPlayers,
            //    Gender = inputModel.Gender,
            //    GameFormat = inputModel.GameFormat,
            //    DurationInHours = inputModel.DurationInHours,
            //    Date = inputModel.Date,
            //    StartingHour = inputModel.StartingHour,
            //    AdditionalInfo = inputModel.AdditionalInfo,
            //    Status = inputModel.Status,
            //    RequestStatus = inputModel.RequestStatus,
            //    ArenaId = inputModel.ArenaId,
            //    AdminId = userId,
            //};

            await this.eventsRepository.AddAsync(@event);
            await this.eventsRepository.SaveChangesAsync();

            await this.eventsUsersRepository.AddAsync(new EventUser
            {
                EventId = @event.Id,
                UserId = userId,
            });

            await this.eventsUsersRepository.SaveChangesAsync();

            return @event.Id;
        }

        public async Task<IEnumerable<T>> GetAll<T>(string currentCity, string currentCountry, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(currentCity, currentCountry);

            IQueryable<Event> query =
                this.eventsRepository.All()
                .Where(e => e.Status != EventStatus.Passed &&
                            e.Status != EventStatus.Full)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .OrderBy(e => e.Date);

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public EventEditViewModel GetDetailsForEdit(int id, (string City, string Country) location)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Id == id)
                .FirstOrDefault();

            var viewModel = ObjectMappingExtensions.To<EventEditViewModel>(query);

            viewModel.Arenas = this.arenasService.GetArenas(location);
            viewModel.Sports = this.sportsService.GetAll();
            return viewModel;
        }

        public async Task UpdateEvent(EventEditViewModel viewModel)
        {

            var @event = this.eventsRepository
                .All()
                .First(e => e.Id == viewModel.Id);

            @event.Name = viewModel.Name;
            @event.MinPlayers = viewModel.MinPlayers;
            @event.MaxPlayers = viewModel.MaxPlayers;
            @event.Gender = viewModel.Gender;
            @event.GameFormat = viewModel.GameFormat;
            @event.DurationInHours = viewModel.DurationInHours;
            @event.Date = viewModel.Date;
            @event.StartingHour = viewModel.StartingHour;
            @event.AdditionalInfo = viewModel.AdditionalInfo;
            @event.Status = viewModel.Status;
            @event.RequestStatus = viewModel.RequestStatus;

            this.eventsRepository.Update(@event);
            await this.eventsRepository.SaveChangesAsync();
        }

        public EventDetailsViewModel GetDetailsWithChatRoom(int id)
        {
            var query = this.eventsRepository.All().Where(e => e.Id == id);

            var viewModel = query.To<EventDetailsViewModel>().FirstOrDefault();

            //var inputModel = this.eventsRepository
            //    .All()
            //    .Where(e => e.Id == id)
            //    .Select(e => new EventDetailsViewModel
            //    {
            //        Id = e.Id,
            //        Name = e.Name,
            //        Arena = e.Arena.Name,
            //        Sport = e.Sport.Name,
            //        Date = e.Date,
            //        Gender = e.Gender,
            //        GameFormat = e.GameFormat,
            //        DurationInHours = e.DurationInHours,
            //        AdditionalInfo = e.AdditionalInfo,
            //        MaxPlayers = e.MaxPlayers,
            //        MinPlayers = e.MinPlayers,
            //        StartingHour = e.StartingHour,
            //        RequestStatus = e.RequestStatus,
            //        Status = e.Status,
            //        AdminUserName = e.Admin.UserName,
            //        AdminUserProfileId = e.Admin.UserProfile.Id,
            //        TotalPrice = e.Arena.PricePerHour * e.DurationInHours,
            //        DeadLineToSendRequest = e.Date.AddDays(-2).ToString("dd.MM.yyyy"),
            //        EmptySpotsLeft = e.MinPlayers - e.Users.Count,
            //        NeededPlayersForConfirmation = e.MinPlayers > e.Users.Count ? e.MinPlayers - e.Users.Count : 0,

            viewModel.ChatRoomMessages = this.messagesService.GetMessagesByEventId(id);

            viewModel.ChatRoomUsers = this.usersProfileService.GetUsersByEventId(id);

            return viewModel;
        }

        public bool IsUserJoined(string username, int eventId) =>
            this.eventsRepository.All()
            .Where(e => e.Id == eventId)
            .Any(e => e.Users.Any(u => u.User.UserName == username));

        public async Task AddUserAsync(int eventId, string userId)
        {
            var eventUser = new EventUser
            {
                EventId = eventId,
                UserId = userId,
            };

            await this.eventsUsersRepository.AddAsync(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();

            await this.ChangeEventStatus(eventId);
        }

        public async Task RemoveUserAsync(int eventId, string userId)
        {
            var eventUser = this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == userId)
                .FirstOrDefault();

            this.eventsUsersRepository.Delete(eventUser);

            await this.ChangeEventStatus(eventId);

            await this.eventsUsersRepository.SaveChangesAsync();
        }

        public HomeEventsListViewModel FilterEventsAsync(EventsFilterInputModel inputModel, (string City, string Country) location)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Arena.Address.City.Country.Name == location.Country)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .Where(e => e.StartingHour.CompareTo(inputModel.From) >= 0 && e.StartingHour.CompareTo(inputModel.To) <= 0);

            if (inputModel.City != "city")
            {
                var cityName = inputModel.City;
                query = query.Where(e => e.Arena.Address.City.Name == cityName);
            }

            if (inputModel.Sport != "sport")
            {
                var sportId = this.sportsService.GetSportId(inputModel.Sport);
                query = query.Where(e => e.SportId == sportId);
            }

            Console.WriteLine(query.Count());

            var events = query.Count();

            var viewModel = new HomeEventsListViewModel
            {
                Events = query
                    .OrderBy(e => e.Date)
                    .Select(q => new HomeEventInfoViewModel
                    {
                        Id = q.Id,
                        ArenaName = q.Arena.Name,
                        SportName = q.Sport.Name,
                        Date = q.Date.ToString("dd-MMM-yyyy") + " at " + q.StartingHour.ToString("hh:mm"),
                        EmptySpotsLeft = q.MaxPlayers - q.Users.Count,
                        SportImage = q.Sport.Image,
                    }).ToList(),
                Cities = query.Select(q => q.Arena.Address.City.Name).ToHashSet(),
                Sports = query.Select(q => q.Sport.Name).ToHashSet(),
                From = inputModel.From,
                To = inputModel.To,
                Sport = inputModel.Sport,
                City = inputModel.City,
            };

            return viewModel;
        }

        public HashSet<string> GetAllSportsInCurrentCountry(string currentCountry)
        {
            var sports = this.eventsRepository
                .All()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Select(e => e.Sport.Name)
                .ToHashSet();

            return sports;
        }

        private async Task SetPassedStatusOnPassedEvents(string currentCity, string currentCountry)
        {
            var eventsToClose = this.eventsRepository
                .All()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Where(e => e.Arena.Address.City.Name == currentCity)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.Date <= DateTime.UtcNow.AddHours(-1));

            if (eventsToClose.Any())
            {
                foreach (var @event in eventsToClose)
                {
                    @event.Status = EventStatus.Passed;
                }

                await this.eventsRepository.SaveChangesAsync();
            }
        }

        private async Task ChangeEventStatus(int eventId)
        {
            var @event = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Id == eventId)
                .FirstOrDefault();

            var currentStatus = @event.Status;

            if (@event.MaxPlayers == @event.Users.Count)
            {
                @event.Status = EventStatus.Full;
            }
            else if (@event.MinPlayers > @event.Users.Count)
            {
                @event.Status = EventStatus.AcceptingPlayers;
            }
            else if (@event.MaxPlayers > @event.Users.Count)
            {
                @event.Status = EventStatus.MinimumPlayersReached;
            }

            if (currentStatus != @event.Status)
            {
                this.eventsRepository.Update(@event);
                await this.eventsRepository.SaveChangesAsync();
            }
        }
    }
}
