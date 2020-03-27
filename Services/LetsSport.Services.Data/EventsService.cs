namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;

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
        private readonly ICountriesService countriesService;
        private readonly IRepository<EventUser> eventsUsersRepository;

        public EventsService(
            ICitiesService citiesService,
            ICountriesService countriesService,
            IArenasService arenasService,
            ISportsService sportsService,
            IMessagesService messagesService,
            IUsersService usersService,
            IEmailSender emailSender,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository)
        {
            this.arenasService = arenasService;
            this.sportsService = sportsService;
            this.messagesService = messagesService;
            this.usersService = usersService;
            this.citiesService = citiesService;
            this.countriesService = countriesService;
            this.emailSender = emailSender;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string userEmail, string username)
        {
            inputModel.CityId = this.citiesService.GetCityIdByArenaId(inputModel.ArenaId);
            inputModel.CountryId = this.countriesService.GetCountryIdByArenaId(inputModel.ArenaId);
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

        public async Task<IEnumerable<T>> GetAllInCity<T>((string City, string Country) location, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(location.Country);

            IQueryable<Event> query =
                this.eventsRepository.All()
                .Where(e => e.Arena.Country.Name == location.Country)
                .Where(e => e.Arena.City.Name == location.City)
                .Where(e => e.Status == EventStatus.AcceptingPlayers ||
                            e.Status == EventStatus.MinimumPlayersReached)
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
            var @event = this.GetEventById(id);
            var viewModel = ObjectMappingExtensions.To<EventEditViewModel>(@event);
            viewModel.Arenas = this.arenasService.GetAllArenas(location);
            viewModel.Sports = this.sportsService.GetAll();

            return viewModel;
        }

        public async Task UpdateEvent(EventEditViewModel viewModel)
        {
            var @event = this.GetEventById(viewModel.Id);

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

        public EventDetailsViewModel GetDetailsWithChatRoom(int id, string userId, string username)
        {
            var query = this.GetEventByIdAsIQuerable(id);

            var viewModel = query.To<EventDetailsViewModel>().FirstOrDefault();

            if (userId != null)
            {
                viewModel.ChatRoom = new ChatRoomPartialViewModel
                {
                    EventId = id,
                    Sport = viewModel.SportName,
                    SportImage = this.sportsService.GetSportImageByName(viewModel.SportName),
                    ChatRoomMessages = this.messagesService.GetMessagesByEventId(id),
                    UserId = userId,
                };

                viewModel.ChatRoomUsers = this.usersService.GetUsersByEventId(id);
            }

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

        public async Task<IEnumerable<T>> GetNotParticipatingEventsInCity<T>(string userId, (string City, string Country) location, int? count = null)
        {
            await this.SetPassedStatusOnPassedEvents(location.Country);

            var query = this.eventsRepository.All()
                .Where(e => !e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Arena.Country.Name == location.Country)
                .Where(e => e.Arena.City.Name == location.City)
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

        public async Task RemoveUserAsync(int eventId, ApplicationUser user)
        {
            var eventUser = this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == user.Id)
                .FirstOrDefault();

            this.eventsUsersRepository.Delete(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateMessageAsync($"Sorry, i have to leave the event!", user.Id, eventId);
            await this.ChangeEventStatus(eventId);

            var eventObject = this.GetEventDetailsForEmailById(eventId);

            await this.emailSender.SendEmailAsync(
                        user.Email,
                        EmailSubjectConstants.LeftEvent,
                        EmailHtmlMessages.GetLeaveEventHtml(user.UserName, eventObject));

            var @event = this.GetEventById(eventId);

            foreach (var player in @event.Users.Where(u => u.User.Id != user.Id))
            {
                await this.emailSender.SendEmailAsync(
                        player.User.Email,
                        EmailSubjectConstants.UserLeft,
                        EmailHtmlMessages.GetUserLeftHtml(player.User.UserName, @event.Sport.Name, @event.Name, @event.Date, user.UserName));
            }
        }

        public async Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, string country)
        {
            await this.SetPassedStatusOnPassedEvents(country);
            var query = this.GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(country, from, to);

            if (city != 0)
            {
                query = query.Where(e => e.Arena.CityId == city);
            }

            if (sport != 0)
            {
                query = query.Where(e => e.SportId == sport);
            }

            IEnumerable<SelectListItem> sports;

            if (city == 0)
            {
                sports = this.sportsService.GetAllSportsInCountry(country);
            }
            else
            {
                var sportsHash = new HashSet<SelectListItem>();

                foreach (var sportKvp in query)
                {
                    sportsHash.Add(new SelectListItem
                    {
                        Text = this.sportsService.GetSportNameById(sportKvp.SportId),
                        Value = sportKvp.SportId.ToString(),
                    });
                }

                sports = sportsHash;
            }

            var events = query.Count();

            // TODO Add Autommaping here !!!!
            var viewModel = new HomeEventsListViewModel
            {
                Events = query
                    .OrderBy(e => e.Date)
                    .Select(q => new EventCardPartialViewModel
                    {
                        Id = q.Id,
                        CityName = q.Arena.City.Name,
                        ArenaName = q.Arena.Name,
                        SportName = q.Sport.Name,
                        Date = q.Date.ToString(GlobalConstants.DefaultDateFormat) +
                               " at " +
                               q.StartingHour.ToString(GlobalConstants.DefaultTimeFormat),
                        EmptySpotsLeft = q.MaxPlayers - q.Users.Count,
                        SportImage = q.Sport.Image,
                        Status = q.Status.GetDisplayName(),
                    }).ToList(),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                    Sports = sports,
                    From = from,
                    To = to,
                    Sport = sport,
                    City = city,
                    Controller = "Home",
                    Action = "Filter",
                },
            };

            return viewModel;
        }

        public async Task<HomeIndexLoggedEventsListViewModel> FilterEventsLoggedAsync(int city, int sport, DateTime from, DateTime to, string userId, string country)
        {
            await this.SetPassedStatusOnPassedEvents(country);
            var query = this.GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(country, from, to)
                .Where(e => e.AdminId != userId)
                .Where(e => !e.Users.Any(u => u.UserId == userId));

            if (city != 0)
            {
                query = query.Where(e => e.Arena.CityId == city);
            }

            if (sport != 0)
            {
                query = query.Where(e => e.SportId == sport);
            }

            IEnumerable<SelectListItem> sports;

            if (city == 0)
            {
                sports = this.sportsService.GetAllSportsInCountry(country);
            }
            else
            {
                var sportsHash = new HashSet<SelectListItem>();

                foreach (var sportKvp in query)
                {
                    sportsHash.Add(new SelectListItem
                    {
                        Text = this.sportsService.GetSportNameById(sportKvp.SportId),
                        Value = sportKvp.Id.ToString(),
                    });
                }

                sports = sportsHash;
            }

            var events = query.Count();

            // TODO Add Autommaping here !!!!
            var viewModel = new HomeIndexLoggedEventsListViewModel
            {
                NotParticipatingEvents = query
                    .OrderBy(e => e.Date)
                    .Select(q => new EventCardPartialViewModel
                    {
                        Id = q.Id,
                        CityName = q.Arena.City.Name,
                        ArenaName = q.Arena.Name,
                        SportName = q.Sport.Name,
                        Date = q.Date.ToString(GlobalConstants.DefaultDateFormat) +
                               " at " +
                               q.StartingHour.ToString(GlobalConstants.DefaultTimeFormat),
                        EmptySpotsLeft = q.MaxPlayers - q.Users.Count,
                        SportImage = q.Sport.Image,
                        Status = q.Status.GetDisplayName(),
                    }).ToList(),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithEventsAsync(country),
                    Sports = sports,
                    From = from,
                    To = to,
                    Sport = sport,
                    City = city,
                    Controller = "Home",
                    Action = "FilterLogged",
                },
            };

            return viewModel;
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

        public async Task<int> InviteUsersToEvent(int id, string email, string userName)
        {
            var serviceModel = this.GetEventAsIQuerableById(id)
                .Select(e => new InviteUsersMessagingModel
                {
                    ArenaName = e.Arena.Name,
                    EventName = e.Name,
                    Sport = e.Sport.Name,
                    Date = e.Date,
                    StartingTime = e.StartingHour,
                    Username = userName,
                    ArenaCityId = e.Arena.CityId,
                })
                .FirstOrDefault();

            var eventLink = $"LetsSport.com/Events/Details/{id}";

            var userEmails = this.usersService.GetAllUsersDetailsForIvitation(serviceModel.Sport, serviceModel.ArenaCityId);
            foreach (var user in userEmails)
            {
                await this.emailSender.SendEmailAsync(
                            user.Email,
                            EmailSubjectConstants.UserInvitation,
                            EmailHtmlMessages.GetUserInvitationHtml(serviceModel, user.Username, eventLink),
                            email,
                            userName);
            }

            return userEmails.Count();
        }

        public bool IsUserJoined(string username, int eventId) =>
           this.eventsRepository.All()
           .Where(e => e.Id == eventId)
           .Any(e => e.Users.Any(u => u.User.UserName == username));

        private IQueryable<Event> GetEventAsIQuerableById(int id)
        {
            return this.eventsRepository
                .All()
                .Where(e => e.Id == id);
        }

        private IQueryable<Event> GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(string country, DateTime from, DateTime to)
        {
            var events = this.eventsRepository.All()
                .Where(e => e.Arena.Country.Name == country)
                .Where(e => e.Status == EventStatus.AcceptingPlayers || e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .Where(e => e.Date.CompareTo(from) >= 0 && e.Date.CompareTo(to) <= 0);

            return events;
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
                .Where(e => e.Arena.Country.Name == currentCountry)
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

            // TODO throw if return null
        }

        private Event GetEventById(int id)
        {
            var @event = this.eventsRepository
                .All()
                .FirstOrDefault(e => e.Id == id);

            if (@event == null)
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            return @event;
        }

        private IQueryable GetEventByIdAsIQuerable(int id)
        {
            var query = this.eventsRepository.All().Where(e => e.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            return query;
        }
    }
}
