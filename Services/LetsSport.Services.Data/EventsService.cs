namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Messaging.Models;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public class EventsService : IEventsService
    {
        private const string InvalidEventIdErrorMessage = "Event with ID: {0} does not exist.";
        private readonly IEmailSender emailSender;
        private readonly IArenasService arenasService;
        private readonly ISportsService sportsService;
        private readonly IMessagesService messagesService;
        private readonly IUsersService usersService;
        private readonly IRepository<Event> eventsRepository;
        private readonly ICitiesService citiesService;
        private readonly IRepository<EventUser> eventsUsersRepository;

        public EventsService(
            IArenasService arenasService,
            ISportsService sportsService,
            IMessagesService messagesService,
            IUsersService usersService,
            ICitiesService citiesService,
            IEmailSender emailSender,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository)
        {
            this.arenasService = arenasService;
            this.sportsService = sportsService;
            this.messagesService = messagesService;
            this.usersService = usersService;
            this.citiesService = citiesService;
            this.emailSender = emailSender;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username)
        {
            var @event = inputModel.To<EventCreateInputModel, Event>(); // ASK NIKI here !!!
            @event.AdminId = userId;
            await this.eventsRepository.AddAsync(@event);
            await this.eventsRepository.SaveChangesAsync();

            await this.eventsUsersRepository.AddAsync(new EventUser
            {
                EventId = @event.Id,
                UserId = userId,
            });

            await this.eventsUsersRepository.SaveChangesAsync();

            var eventId = @event.Id;
            await this.messagesService.CreateMessageAsync($"{inputModel.Name} has been created!", userId, eventId);

            var sportName = this.sportsService.GetSportNameById(inputModel.SportId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.EventCreated,
                        EmailHtmlMessages.GetEventCreationHtml(
                            username,
                            inputModel.Name,
                            sportName,
                            inputModel.Date.ToString(GlobalConstants.DefaultDateFormat),
                            inputModel.StartingHour.ToString(GlobalConstants.DefaultTimeFormat)));

            return eventId;
        }

        public async Task<IEnumerable<T>> GetAll<T>(string country, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(country);

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

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            var viewModel = ObjectMappingExtensions.To<EventEditViewModel>(query);

            viewModel.Arenas = this.arenasService.GetAllArenas(location);

            viewModel.Sports = this.sportsService.GetAll();

            return viewModel;
        }

        public async Task UpdateEvent(EventEditViewModel viewModel)
        {
            var @event = this.eventsRepository
                .All()
                .First(e => e.Id == viewModel.Id);

            if (@event == null)
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, viewModel.Id));
            }

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

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            var viewModel = query.To<EventDetailsViewModel>().FirstOrDefault();
            viewModel.ChatRoomMessages = this.messagesService.GetMessagesByEventId(id);
            viewModel.ChatRoomUsers = this.usersService.GetUsersByEventId(id);

            return viewModel;
        }

        public async Task<IEnumerable<T>> GetAllAdministratingEventsByUserId<T>(string userId, string country, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Canceled)
                .Where(e => e.Status != EventStatus.Passed);

            if (count.HasValue)
            {
                return query.OrderBy(e => e.Date).Take(count.Value).To<T>().ToList();
            }
            else
            {
                return query.OrderBy(e => e.Date).To<T>().ToList();
            }
        }

        public async Task<IEnumerable<T>> GetUpcomingEvents<T>(string userId, string country, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status != EventStatus.Passed && e.Status != EventStatus.Canceled);

            if (count.HasValue)
            {
                return query.OrderBy(e => e.Date).Take(count.Value).To<T>().ToList();
            }
            else
            {
                return query.OrderBy(e => e.Date).To<T>().ToList();
            }
        }

        public async Task<IEnumerable<T>> GetNotParticipatingEvents<T>(string userId, string country, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => !e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Arena.Address.City.Country.Name == country)
                .Where(e => e.Status != EventStatus.Passed && e.Status != EventStatus.Full && e.Status != EventStatus.Canceled)
                .Where(e => e.MaxPlayers > e.Users.Count);

            if (count.HasValue)
            {
                return query.OrderBy(e => e.Date).Take(count.Value).To<T>().ToList();
            }
            else
            {
                return query.OrderBy(e => e.Date).To<T>().ToList();
            }
        }

        public async Task<IEnumerable<T>> GetCanceledEvents<T>(string userId, string country, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Passed && e.Status == EventStatus.Canceled);

            if (count.HasValue)
            {
                return query.OrderBy(e => e.Date).Take(count.Value).To<T>().ToList();
            }
            else
            {
                return query.OrderBy(e => e.Date).To<T>().ToList();
            }
        }

        public async Task AddUserAsync(int eventId, string userId, string userEmail, string username)
        {
            // TODO validate id's
            var eventUser = new EventUser
            {
                EventId = eventId,
                UserId = userId,
            };

            await this.eventsUsersRepository.AddAsync(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateMessageAsync($"Hi, I just joined the event!", userId, eventId);
            await this.ChangeEventStatus(eventId);

            var eventObject = this.GetEventDetailsForEmailById(eventId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.JoinedEvent,
                        EmailHtmlMessages.GetJoinEventHtml(username, eventObject));
        }

        public async Task RemoveUserAsync(int eventId, string userId, string userEmail, string username)
        {
            // TODO validate Id's
            // TODO send emails to all event users.
            var eventUser = this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == userId)
                .FirstOrDefault();

            this.eventsUsersRepository.Delete(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateMessageAsync($"Sorry, i have to leave the event!", userId, eventId);
            await this.ChangeEventStatus(eventId);

            var eventObject = this.GetEventDetailsForEmailById(eventId);

            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.LeftEvent,
                        EmailHtmlMessages.GetLeaveEventHtml(username, eventObject));

            var @event = this.GetEventById(eventId);

            foreach (var user in @event.Users.Where(u => u.User.Id != userId))
            {
                await this.emailSender.SendEmailAsync(
                        user.User.Email,
                        EmailSubjectConstants.UserLeft,
                        EmailHtmlMessages.GetUserLeftHtml(user.User.UserName, @event.Sport.Name, @event.Name, @event.Date, username));
            }
        }

        public async Task<HomeEventsListViewModel> FilterEventsAsync(EventsFilterInputModel inputModel, string country)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => e.Arena.Address.City.Country.Name == country)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .Where(e => e.Date.CompareTo(inputModel.From) >= 0 && e.Date.CompareTo(inputModel.To) <= 0);

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

            var events = query.Count();

            // TODO Add Autommaping here !!!!
            var viewModel = new HomeEventsListViewModel
            {
                Events = query
                    .OrderBy(e => e.Date)
                    .Select(q => new HomeEventInfoViewModel
                    {
                        Id = q.Id,
                        ArenaAddressCityName = q.Arena.Address.City.Name,
                        ArenaName = q.Arena.Name,
                        SportName = q.Sport.Name,
                        Date = q.Date.ToString(GlobalConstants.DefaultDateFormat) +
                               " at " +
                               q.StartingHour.ToString(GlobalConstants.DefaultTimeFormat),
                        EmptySpotsLeft = q.MaxPlayers - q.Users.Count,
                        SportImage = q.Sport.Image,
                    }).ToList(),
                Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                Sports = query.Select(q => q.Sport.Name).ToHashSet(),
                From = inputModel.From,
                To = inputModel.To,
                Sport = inputModel.Sport,
                City = inputModel.City,
                Country = country,
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

        public async Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId, string country)
        {
            var events = await this.GetEventsByArenaAdminId<ArenaEventsEventInfoViewModel>(userId, country);

            var viewModel = new ArenaEventsViewModel
            {
                TodaysEvents = events
                    .Where(e => e.Date == DateTime.UtcNow)
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved.ToString())
                    .ToList(),
                ApprovedEvents = events
                    .Where(e => e.Date > DateTime.UtcNow).ToList()
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved.ToString())
                    .ToList(),
                NotApporvedEvents = events
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.NotApproved.ToString())
                    .ToList(),
            };

            return viewModel;
        }

        public bool IsUserJoined(string username, int eventId) =>
            this.eventsRepository.All()
            .Where(e => e.Id == eventId)
            .Any(e => e.Users.Any(u => u.User.UserName == username));

        public async Task<HomeIndexLoggedEventsListViewModel> FilterEventsLoggedAsync(EventsFilterInputModel inputModel, string userId, string country)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository.All()
                .Where(e => e.Arena.Address.City.Country.Name == country)
                .Where(e => e.AdminId != userId)
                .Where(e => !e.Users.Any(u => u.UserId == userId))
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .Where(e => e.Date.CompareTo(inputModel.From) >= 0 && e.Date.CompareTo(inputModel.To) <= 0);

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

            var events = query.Count();

            // TODO Add Autommaping here !!!!
            var viewModel = new HomeIndexLoggedEventsListViewModel
            {
                NotParticipatingEvents = query
                    .OrderBy(e => e.Date)
                    .Select(q => new HomeEventInfoViewModel
                    {
                        Id = q.Id,
                        ArenaAddressCityName = q.Arena.Address.City.Name,
                        ArenaName = q.Arena.Name,
                        SportName = q.Sport.Name,
                        Date = q.Date.ToString(GlobalConstants.DefaultDateFormat) +
                               " at " +
                               q.StartingHour.ToString(GlobalConstants.DefaultTimeFormat),
                        EmptySpotsLeft = q.MaxPlayers - q.Users.Count,
                        SportImage = q.Sport.Image,
                    }).ToList(),
                Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                Sports = query.Select(q => q.Sport.Name).ToHashSet(),
                From = inputModel.From,
                To = inputModel.To,
                Sport = inputModel.Sport,
                City = inputModel.City,
            };

            return viewModel;
        }

        public async Task CancelEvent(int id, string userEmail, string username)
        {
            var @event = this.GetEventById(id);
            @event.Status = EventStatus.Canceled;
            this.eventsRepository.Update(@event);
            await this.eventsRepository.SaveChangesAsync();

            var eventUsers = this.eventsUsersRepository
                .All()
                .Where(eu => eu.EventId == id)
                .ToList();

            for (int i = 0; i < eventUsers.Count; i++)
            {
                this.eventsUsersRepository.Delete(eventUsers[i]);
            }

            await this.eventsUsersRepository.SaveChangesAsync();

            var sportName = this.sportsService.GetSportNameById(@event.SportId);

            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.CancelEvent,
                        EmailHtmlMessages.GetCancelEventHtml(username, sportName, @event.Name, @event.Date));

            foreach (var user in @event.Users.Where(u => u.User.Email != userEmail))
            {
                await this.emailSender.SendEmailAsync(
                        user.User.Email,
                        EmailSubjectConstants.EventCanceled,
                        EmailHtmlMessages.GetEventCanceledHtml(user.User.UserName, @event.Admin.UserName, sportName, @event.Name, @event.Date));
            }
        }

        private async Task ChangeEventStatus(int eventId)
        {
            var @event = this.GetEventById(eventId);

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

                foreach (var user in @event.Users)
                {
                    await this.emailSender.SendEmailAsync(
                        user.User.Email,
                        EmailSubjectConstants.ChangedStatus,
                        EmailHtmlMessages.GetChangedStatusHtml(user.User.UserName, @event.Sport.Name, @event.Name, @event.Date, currentStatus.ToString()));
                }
            }
        }

        private async Task SetPassedStatusOnPassedEvents(string currentCountry)
        {
            var eventsToClose = this.eventsRepository
                .All()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.Status != EventStatus.Canceled)
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

        private async Task<IEnumerable<T>> GetEventsByArenaAdminId<T>(string adminId, string country)
        {
            await this.SetPassedStatusOnPassedEvents(country);

            var query = this.eventsRepository
                .All()
                .Where(e => e.Arena.ArenaAdminId == adminId)
                .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved ||
                            e.ArenaRequestStatus == ArenaRentalRequestStatus.NotApproved)
                .OrderBy(e => e.Date);

            return query.To<T>().ToList();
        }

        private EventDetailsModel GetEventDetailsForEmailById(int eventId)
        {
            return this.eventsRepository
                .All()
                .Where(e => e.Id == eventId)
                .Select(e => new EventDetailsModel
                {
                    Name = e.Name,
                    Arena = e.Arena.Name,
                    Orginizer = e.Admin.UserName,
                    Date = e.Date,
                    Time = e.StartingHour,
                })
                .FirstOrDefault();
        }

        private Event GetEventById(int id)
        {
            return this.eventsRepository
                .All()
                .FirstOrDefault(e => e.Id == id);
        }
    }
}
