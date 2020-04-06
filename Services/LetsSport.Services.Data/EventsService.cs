namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Events;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class EventsService : IEventsService
    {
        private const string InvalidEventIdErrorMessage = "Event with ID: {0} does not exist.";
        private readonly IEmailSender emailSender;
        private readonly ICountriesService countriesService;
        private readonly IArenasService arenasService;
        private readonly ISportsService sportsService;
        private readonly IMessagesService messagesService;
        private readonly IUsersService usersService;
        private readonly IRepository<Event> eventsRepository;
        private readonly ICitiesService citiesService;
        private readonly IRepository<EventUser> eventsUsersRepository;

        public EventsService(
            ICitiesService citiesService,
            IArenasService arenasService,
            ISportsService sportsService,
            IMessagesService messagesService,
            IUsersService usersService,
            IEmailSender emailSender,
            ICountriesService countriesService,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository)
        {
            this.arenasService = arenasService;
            this.sportsService = sportsService;
            this.messagesService = messagesService;
            this.usersService = usersService;
            this.citiesService = citiesService;
            this.emailSender = emailSender;
            this.countriesService = countriesService;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
        }

        public async Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId, int? count = null)
        {
            IQueryable<Event> query =
                this.eventsRepository.All()
                .Where(e => e.CityId == cityId)
                .Where(e => e.Status == EventStatus.AcceptingPlayers ||
                            e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.StartingHour.Hour);

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAdministratingByUserIdAsync<T>(string userId, int? count = null)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Canceled
                         && e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Failed);

            if (count.HasValue)
            {
                return await query
                    .OrderBy(e => e.Date)
                    .Take(count.Value)
                    .To<T>()
                    .ToListAsync();
            }
            else
            {
                return await query
                    .OrderBy(e => e.Date)
                    .To<T>()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllUpcomingByUserIdAsync<T>(string userId, int? count = null)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Canceled
                         && e.Status != EventStatus.Failed);

            if (count.HasValue)
            {
                return await query
                    .OrderBy(e => e.Date)
                    .Take(count.Value)
                    .To<T>()
                    .ToListAsync();
            }
            else
            {
                return await query
                    .OrderBy(e => e.Date)
                    .To<T>()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> GetNotParticipatingInCityAsync<T>(string userId, int cityId, int? count = null)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Arena.CityId == cityId)
                .Where(e => !e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Full
                         && e.Status != EventStatus.Canceled
                         && e.Status != EventStatus.Failed)
                .Where(e => e.MaxPlayers > e.Users.Count);

            if (count.HasValue)
            {
                return await query
                    .OrderBy(e => e.Date)
                    .Take(count.Value)
                    .To<T>()
                    .ToListAsync();
            }
            else
            {
                return await query
                    .OrderBy(e => e.Date)
                    .To<T>()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAdminAllCanceledAsync<T>(string userId, int? count = null)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Failed
                         && e.Status == EventStatus.Canceled);

            if (count.HasValue)
            {
                return await query
                    .OrderBy(e => e.Date)
                    .Take(count.Value)
                    .To<T>()
                    .ToListAsync();
            }
            else
            {
                return await query
                    .OrderBy(e => e.Date)
                    .To<T>()
                    .ToListAsync();
            }
        }

        public async Task SetPassedStatusAsync(int countryId)
        {
            var eventsToClose = this.eventsRepository
                .All()
                .Where(e => e.Arena.CountryId == countryId)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.Date <= DateTime.UtcNow.AddHours(-1));

            if (eventsToClose.Any())
            {
                await eventsToClose.ForEachAsync(e => e.Status = EventStatus.Passed);
                await this.eventsRepository.SaveChangesAsync();
            }
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
            await this.messagesService.CreateAsync($"{inputModel.Name} has been created!", userId, eventId);

            var sportName = this.sportsService.GetNameById(inputModel.SportId);
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

        public async Task<EventDetailsViewModel> GetDetailsAsync(int id, string userId)
        {
            var query = this.GetAsIQuerableById(id);

            var viewModel = await query.To<EventDetailsViewModel>().FirstOrDefaultAsync();

            if (userId != null)
            {
                viewModel.ChatRoomUsers = this.usersService.GetAllByEventId(id);
            }

            return viewModel;
        }

        public async Task<EventEditViewModel> GetDetailsForEditAsync(int id)
        {
            var viewModel = this.GetAsIQuerableById(id).To<EventEditViewModel>().FirstOrDefault();
            viewModel.Arenas = await this.arenasService.GetAllActiveInCitySelectListAsync(viewModel.CityId);
            viewModel.Sports = this.sportsService.GetAllAsSelectList();

            return viewModel;
        }

        public async Task UpdateAsync(EventEditViewModel viewModel)
        {
            var @event = this.GetAsIQuerableById(viewModel.Id).First();

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
            @event.SportId = viewModel.SportId;

            this.eventsRepository.Update(@event);
            await this.eventsRepository.SaveChangesAsync();
        }

        public async Task CancelEvent(int id, string userEmail, string username)
        {
            var @event = this.GetAsIQuerableById(id).First();

            if (@event == null)
            {
                throw new ArgumentException($"Event with ID: {id} does not exists!");
            }

            @event.Status = EventStatus.Canceled;
            this.eventsRepository.Update(@event);
            await this.eventsRepository.SaveChangesAsync();

            // TOTO do i need that ?
            var eventUsers = this.eventsUsersRepository
                .All()
                .Where(eu => eu.EventId == id)
                .ToList();

            for (int i = 0; i < eventUsers.Count; i++)
            {
                this.eventsUsersRepository.Delete(eventUsers[i]);
            }

            await this.eventsUsersRepository.SaveChangesAsync();

            var sportName = this.sportsService.GetNameById(@event.SportId);

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

        public async Task AddUserAsync(int eventId, string userId, string userEmail, string username)
        {
            var eventUser = new EventUser
            {
                EventId = eventId,
                UserId = userId,
            };

            await this.eventsUsersRepository.AddAsync(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateAsync($"Hi, I just joined the event!", userId, eventId);
            await this.ChangeEventStatus(eventId);

            var eventObject = this.GetEventDetailsForEmailById(eventId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.JoinedEvent,
                        EmailHtmlMessages.GetJoinEventHtml(username, eventObject));
        }

        public async Task<int> InviteUsersToEvent(int id, string email, string userName)
        {
            var serviceModel = this.GetAsIQuerableById(id)
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

        public async Task RemoveUserAsync(int eventId, string userId, string username, string email)
        {
            var eventUser = this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == userId)
                .FirstOrDefault();

            if (eventUser == null)
            {
                throw new ArgumentException($"User with that ID: {userId} does not participate in event with ID: {eventId}");
            }

            this.eventsUsersRepository.Delete(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateAsync($"Sorry, i have to leave the event!", userId, eventId);
            await this.ChangeEventStatus(eventId);

            var eventObject = this.GetEventDetailsForEmailById(eventId);

            await this.emailSender.SendEmailAsync(
                        email,
                        EmailSubjectConstants.LeftEvent,
                        EmailHtmlMessages.GetLeaveEventHtml(username, eventObject));

            var @event = this.GetAsIQuerableById(eventId).First();

            foreach (var player in @event.Users.Where(u => u.User.Id != userId))
            {
                await this.emailSender.SendEmailAsync(
                        player.User.Email,
                        EmailSubjectConstants.UserLeft,
                        EmailHtmlMessages.GetUserLeftHtml(player.User.UserName, @event.Sport.Name, @event.Name, @event.Date, username));
            }
        }

        public async Task<HomeEventsListViewModel> FilterEventsAsync(int city, int sport, DateTime from, DateTime to, int countryId, string userId)
        {
            var query = this.GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(countryId, from, to);

            if (userId != null)
            {
                query = query
                    .Where(e => !e.Users.Any(u => u.UserId == userId));
            }

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
                sports = await this.sportsService.GetAllInCountryByIdAsync(countryId);
            }
            else
            {
                sports = query
                   .Select(a => new SelectListItem
                   {
                       Text = this.sportsService.GetNameById(a.SportId),
                       Value = a.SportId.ToString(),
                   })
                   .Distinct();
            }

            var viewModel = new HomeEventsListViewModel
            {
                Events = await query.To<EventCardPartialViewModel>().ToListAsync(),
                Filter = new FilterBarPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithEventsInCountryAsync(countryId),
                    Sports = sports,
                    From = from,
                    To = to,
                    Sport = sport,
                    City = city,
                },
            };

            return viewModel;
        }

        public async Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(string userId)
        {
            var events = await this.GetEventsByArenaAdminId<ArenaEventsEventInfoViewModel>(userId);

            var viewModel = new ArenaEventsViewModel
            {
                TodaysEvents = events
                    .Where(e => e.Date == DateTime.UtcNow)
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved.ToString())
                    .ToList(),
                ApprovedEvents = events
                    .Where(e => e.Date > DateTime.UtcNow)
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved.ToString())
                    .ToList(),
                NotApporvedEvents = events
                    .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.NotApproved.ToString())
                    .ToList(),
            };

            return viewModel;
        }

        public bool IsUserJoined(string userId, int eventId) =>
           this.eventsRepository.All()
           .Where(e => e.Id == eventId)
           .Any(e => e.Users.Any(u => u.User.Id == userId));

        // Admin
        public async Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId)
        {
            return await this.eventsRepository
                .All()
                .Where(e => e.CountryId == countryId)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IndexViewModel> FilterAsync(int countryId, int? cityId, int? sportId)
        {
            var query = this.eventsRepository
                .All()
                .Where(e => e.CountryId == countryId)
                .Where(e => e.Status != EventStatus.Passed);

            if (cityId != null)
            {
                query = query
                    .Where(a => a.CityId == cityId);
            }

            if (sportId != null)
            {
                query = query
                    .Where(a => a.SportId == sportId);
            }

            var events = query
                 .OrderBy(e => e.Date)
                 .ThenBy(e => e.StartingHour.Hour)
                 .ThenBy(e => e.City.Name)
                 .ThenBy(e => e.Name)
                 .To<InfoViewModel>()
                 .ToList();

            var countryName = this.countriesService.GetNameById(countryId);
            var location = cityId != null
                ? this.citiesService.GetNameById(cityId.Value) + ", " + countryName
                : countryName;

            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Events = events,
                Location = location,
                Filter = new FilterBarViewModel
                {
                    City = cityId,
                    Sport = sportId,
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return viewModel;
        }

        public T GetEventById<T>(int id)
        {
            return this.GetAsIQuerableById(id)
                .To<T>()
                .FirstOrDefault();
        }

        public async Task AdminUpdateAsync(EditViewModel inputModel)
        {
            var evt = this.GetAsIQuerableById(inputModel.Id).First();

            evt.Name = inputModel.Name;
            evt.SportId = inputModel.SportId;
            evt.ArenaId = inputModel.ArenaId;
            evt.Date = inputModel.Date;
            evt.StartingHour = inputModel.StartingHour;
            evt.DurationInHours = inputModel.DurationInHours;
            evt.Gender = inputModel.Gender;
            evt.GameFormat = inputModel.GameFormat;
            evt.MinPlayers = inputModel.MinPlayers;
            evt.MaxPlayers = inputModel.MaxPlayers;
            evt.Status = inputModel.Status;
            evt.RequestStatus = inputModel.RequestStatus;
            evt.AdditionalInfo = inputModel.AdditionalInfo;
            evt.AdminId = inputModel.AdminId;

            this.eventsRepository.Update(evt);
            await this.eventsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var @event = this.GetAsIQuerableById(id).First();
            this.eventsRepository.Delete(@event);
            await this.eventsRepository.SaveChangesAsync();
        }

        private IQueryable<Event> GetAsIQuerableById(int id)
        {
            var query = this.eventsRepository
                .All()
                .Where(e => e.Id == id);

            if (!query.Any())
            {
                throw new ArgumentNullException(string.Format(InvalidEventIdErrorMessage, id));
            }

            return query;
        }

        private IQueryable<Event> GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(int countryId, DateTime from, DateTime to)
        {
            var events = this.eventsRepository.All()
                .Where(e => e.Arena.CountryId == countryId)
                .Where(e => e.Status == EventStatus.AcceptingPlayers || e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .Where(e => e.Date.CompareTo(from) >= 0 && e.Date.CompareTo(to) <= 0)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.StartingHour.Hour);

            return events;
        }

        private async Task ChangeEventStatus(int eventId)
        {
            var @event = this.GetAsIQuerableById(eventId).First();

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
                        EmailHtmlMessages.GetChangedStatusHtml(user.User.UserName, @event.Sport.Name, @event.Name, @event.Date, currentStatus.GetDisplayName()));
                }
            }
        }

        private async Task<IEnumerable<T>> GetEventsByArenaAdminId<T>(string adminId)
        {
            var query = this.eventsRepository
                .All()
                .Where(e => e.Arena.ArenaAdminId == adminId)
                .Where(e => e.ArenaRequestStatus == ArenaRentalRequestStatus.Approved ||
                            e.ArenaRequestStatus == ArenaRentalRequestStatus.NotApproved)
                .OrderBy(e => e.Date);

            return await query.To<T>().ToListAsync();
        }

        private EventDetailsModel GetEventDetailsForEmailById(int eventId)
        {
            return this.GetAsIQuerableById(eventId)
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
    }
}
