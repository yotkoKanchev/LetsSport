namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Services.Models;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Events;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;
    using static LetsSport.Common.GlobalConstants;

    public class EventsService : IEventsService
    {
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

        public async Task<IEnumerable<T>> GetAllInCityAsync<T>(int countryId, int cityId, int? take = null, int skip = 0)
        {
            await this.SetPassedStatusAsync(countryId);
            var query = this.eventsRepository.All()
                .Where(e => e.CityId == cityId)
                .Where(e => e.Status == EventStatus.AcceptingPlayers ||
                            e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.StartingHour.Hour)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<int> GetCountInCityAsync(int cityId)
        {
            return await this.eventsRepository
                .All()
                .Where(e => e.CityId == cityId)
                .Where(e => e.Status == EventStatus.AcceptingPlayers ||
                            e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .CountAsync();
        }

        public async Task<IEnumerable<T>> GetAllAdministratingByUserIdAsync<T>(
            int countryId, string userId, int? take = null)
        {
            await this.SetPassedStatusAsync(countryId);
            IQueryable<Event> query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Canceled
                         && e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Failed)
                .OrderBy(e => e.Date);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllUpcomingByUserIdAsync<T>(int countryId, string userId, int? take = null)
        {
            await this.SetPassedStatusAsync(countryId);
            IQueryable<Event> query = this.eventsRepository.All()
                .Where(e => e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Canceled
                         && e.Status != EventStatus.Failed)
                .OrderBy(e => e.Date);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetNotParticipatingInCityAsync<T>(
            int countryId, string userId, int cityId, int? take = null, int skip = 0)
        {
            await this.SetPassedStatusAsync(countryId);
            IQueryable<Event> query = this.eventsRepository.All()
                .Where(e => e.Arena.CityId == cityId)
                .Where(e => !e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status == EventStatus.AcceptingPlayers
                         || e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .OrderBy(e => e.Date)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<int> GetNotParticipatingCount(string userId, int cityId)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Arena.CityId == cityId)
                .Where(e => !e.Users
                    .Any(u => u.UserId == userId))
                .Where(e => e.Status == EventStatus.AcceptingPlayers
                         || e.Status == EventStatus.MinimumPlayersReached)
                .Where(e => e.MaxPlayers > e.Users.Count);

            return await query.CountAsync();
        }

        public async Task<IEnumerable<T>> GetAdminAllCanceledAsync<T>(string userId, int? take = null)
        {
            IQueryable<Event> query = this.eventsRepository.All()
                .Where(e => e.AdminId == userId)
                .Where(e => e.Status != EventStatus.Passed
                         && e.Status != EventStatus.Failed
                         && e.Status == EventStatus.Canceled)
                .OrderByDescending(e => e.Date);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<int> CreateAsync(
            EventCreateInputModel inputModel, int cityId, int countryId, string userId, string userEmail, string username)
        {
            var @event = inputModel.To<Event>();
            @event.CityId = cityId;
            @event.CountryId = countryId;
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
            await this.messagesService.CreateAsync(EventCreationMessage, userId, eventId);

            var sportName = await this.sportsService.GetNameByIdAsync(inputModel.SportId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.EventCreated,
                        EmailHtmlMessages.GetEventCreationHtml(
                            username,
                            inputModel.Name,
                            sportName,
                            inputModel.Date.ToString(DefaultDateFormat),
                            inputModel.StartingHour.ToString(DefaultTimeFormat)));

            return eventId;
        }

        public async Task<EventDetailsViewModel> GetDetailsAsync(int id, string userId)
        {
            var query = this.GetAsIQuerableById(id);

            if (!query.Any())
            {
                return null;
            }

            var viewModel = await query.To<EventDetailsViewModel>().FirstOrDefaultAsync();

            if (userId != null)
            {
                viewModel.ChatRoomUsers = await this.usersService.GetAllByEventIdAsync(id);
            }

            return viewModel;
        }

        public async Task<EventEditViewModel> GetDetailsForEditAsync(int id)
        {
            var viewModel = await this.GetAsIQuerableById(id).To<EventEditViewModel>().FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return null;
            }

            viewModel.Arenas = await this.arenasService.GetAllActiveInCitySelectListAsync(viewModel.CityId);
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();

            return viewModel;
        }

        public async Task UpdateAsync(EventEditViewModel viewModel)
        {
            var evt = await this.GetAsIQuerableById(viewModel.Id).FirstOrDefaultAsync();

            if (evt != null)
            {
                evt.Name = viewModel.Name;
                evt.MinPlayers = viewModel.MinPlayers;
                evt.MaxPlayers = viewModel.MaxPlayers;
                evt.Gender = viewModel.Gender;
                evt.GameFormat = viewModel.GameFormat;
                evt.DurationInHours = viewModel.DurationInHours;
                evt.Date = viewModel.Date;
                evt.StartingHour = viewModel.StartingHour;
                evt.AdditionalInfo = viewModel.AdditionalInfo;
                evt.Status = viewModel.Status;
                evt.RequestStatus = viewModel.RequestStatus;
                evt.SportId = viewModel.SportId;

                this.eventsRepository.Update(evt);
                await this.eventsRepository.SaveChangesAsync();
            }
        }

        public async Task CancelEventAsync(int eventId, string userEmail, string username)
        {
            var evt = await this.GetAsIQuerableById(eventId).FirstOrDefaultAsync();

            if (evt != null)
            {
                evt.Status = EventStatus.Canceled;
                this.eventsRepository.Update(evt);
                await this.eventsRepository.SaveChangesAsync();

                var eventUsers = await this.eventsUsersRepository
                    .All()
                    .Where(eu => eu.EventId == eventId)
                    .ToListAsync();

                for (int i = 0; i < eventUsers.Count; i++)
                {
                    this.eventsUsersRepository.Delete(eventUsers[i]);
                }

                await this.eventsUsersRepository.SaveChangesAsync();

                var sportName = await this.sportsService.GetNameByIdAsync(evt.SportId);

                await this.emailSender.SendEmailAsync(
                            userEmail,
                            EmailSubjectConstants.CancelEvent,
                            EmailHtmlMessages.GetCancelEventHtml(username, sportName, evt.Name, evt.Date));

                var users = await this.eventsUsersRepository
                    .All()
                    .Where(eu => eu.EventId == eventId)
                    .Select(eu => new EmailUserInfo { Email = eu.User.Email, Username = eu.User.UserName })
                    .ToListAsync();

                foreach (var user in users)
                {
                    await this.emailSender.SendEmailAsync(
                            user.Email,
                            EmailSubjectConstants.EventCanceled,
                            EmailHtmlMessages.GetEventCanceledHtml(user.Username, evt.Admin.UserName, sportName, evt.Name, evt.Date));
                }
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
            await this.messagesService.CreateAsync(EventJoiningMessage, userId, eventId);
            await this.ChangeEventStatusAsync(eventId);

            var eventObject = await this.GetEventDetailsForEmailByIdAsync(eventId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.JoinedEvent,
                        EmailHtmlMessages.GetJoinEventHtml(username, eventObject));
        }

        public async Task<int> InviteUsersToEventAsync(int id, string email, string userName)
        {
            var query = this.GetAsIQuerableById(id);

            if (query.Any())
            {
                var serviceModel = await query
                    .Select(e => new InviteUsersMessagingModel
                    {
                        ArenaName = e.Arena.Name,
                        EventName = e.Name,
                        Sport = e.Sport.Name,
                        SportId = e.Sport.Id,
                        Date = e.Date,
                        StartingTime = e.StartingHour,
                        Username = userName,
                        ArenaCityId = e.Arena.CityId,
                    })
                    .FirstOrDefaultAsync();

                var eventLink = $"LetsSport.com/Events/Details/{id}";
                var userEmails = await this.usersService.GetAllUsersDetailsForIvitationAsync(
                    serviceModel.SportId, serviceModel.ArenaCityId);

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

            return 0;
        }

        public async Task RemoveUserAsync(int eventId, string userId, string username, string email)
        {
            var eventUser = await this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == userId)
                .FirstOrDefaultAsync();

            if (eventUser == null)
            {
                throw new ArgumentException(string.Format(EventIvanlidIdwithUserIdErrorMessage, userId, eventId));
            }

            this.eventsUsersRepository.Delete(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
            await this.messagesService.CreateAsync(EventLeavingMessage, userId, eventId);
            await this.ChangeEventStatusAsync(eventId);

            var eventObject = await this.GetEventDetailsForEmailByIdAsync(eventId);

            await this.emailSender.SendEmailAsync(
                        email,
                        EmailSubjectConstants.LeftEvent,
                        EmailHtmlMessages.GetLeaveEventHtml(username, eventObject));

            var users = await this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId)
                .Select(eu => new EmailUserInfo { Email = eu.User.Email, Username = eu.User.UserName, })
                .ToListAsync();

            foreach (var player in users)
            {
                await this.emailSender.SendEmailAsync(
                        player.Email,
                        EmailSubjectConstants.UserLeft,
                        EmailHtmlMessages.GetUserLeftHtml(
                            player.Username, eventObject.Sport, eventObject.Name, eventObject.Date, username));
            }
        }

        public async Task<HomeEventsListViewModel> FilterAsync(
            int? cityId, int? sportId, DateTime from, DateTime to, int countryId, string userId, int? take = null, int skip = 0)
        {
            await this.SetPassedStatusAsync(countryId);
            var query = this.GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(countryId, from, to);

            if (userId != null)
            {
                query = query
                    .Where(e => !e.Users.Any(u => u.UserId == userId));
            }

            if (cityId != null)
            {
                query = query.Where(e => e.CityId == cityId);
            }

            if (sportId != null)
            {
                query = query.Where(e => e.SportId == sportId);
            }

            var resultCount = query.Count();
            IEnumerable<SelectListItem> sports;

            if (cityId == null || resultCount == 0)
            {
                sports = await this.sportsService.GetAllInCountryByIdAsync(countryId);
            }
            else
            {
                sports = query
                   .Select(a => new SelectListItem
                   {
                       Text = this.sportsService.GetNameByIdAsync(a.SportId).GetAwaiter().GetResult(),
                       Value = a.SportId.ToString(),
                   })
                   .Distinct();
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var viewModel = new HomeEventsListViewModel
            {
                Events = await query.To<EventCardPartialViewModel>().ToListAsync(),
                CityId = cityId,
                SportId = sportId,
                From = from,
                To = to,
                ResultCount = resultCount,
                Filter = new FilterBarPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithEventsInCountryAsync(countryId),
                    Sports = sports,
                    From = from,
                    To = to,
                    SportId = sportId,
                    CityId = cityId,
                },
            };

            var countryName = await this.countriesService.GetNameByIdAsync(countryId);
            viewModel.Location = cityId != null
                ? await this.citiesService.GetNameByIdAsync(cityId.Value) + ", " + countryName
                : $"{countryName}";

            return viewModel;
        }

        public async Task<ArenaEventsViewModel> GetArenaEventsByArenaAdminId(int countryId, string userId)
        {
            await this.SetPassedStatusAsync(countryId);
            var events = this.GetEventsByArenaAdminIdAsIQueryable<ArenaEventsEventInfoViewModel>(userId);

            var viewModel = new ArenaEventsViewModel
            {
                TodaysEvents = await events
                    .Where(e => e.Date.Date == DateTime.UtcNow.Date)
                    .Where(e => e.ArenaRentalRequestStatus == ArenaRentalRequestStatus.Approved)
                    .ToListAsync(),
                ApprovedEvents = await events
                    .Where(e => e.Date.Date > DateTime.UtcNow.Date)
                    .Where(e => e.ArenaRentalRequestStatus == ArenaRentalRequestStatus.Approved)
                    .ToListAsync(),
                NotApporvedEvents = await events
                    .Where(e => e.Date.Date >= DateTime.UtcNow.Date)
                    .Where(e => e.ArenaRentalRequestStatus == ArenaRentalRequestStatus.NotApproved)
                    .ToListAsync(),
            };

            return viewModel;
        }

        public async Task ChangeStatus(int eventId, ArenaRequestStatus status)
        {
            var evt = await this.GetAsIQuerableById(eventId).FirstOrDefaultAsync();

            if (evt != null)
            {
                evt.RequestStatus = status;

                this.eventsRepository.Update(evt);
                await this.eventsRepository.SaveChangesAsync();

                var sportName = await this.sportsService.GetNameByIdAsync(evt.SportId);
                var users = await this.eventsUsersRepository.All()
                    .Where(eu => eu.EventId == eventId)
                    .Select(eu => new EmailUserInfo { Email = eu.User.Email, Username = eu.User.UserName, })
                    .ToListAsync();

                foreach (var player in users)
                {
                    await this.emailSender.SendEmailAsync(
                            player.Email,
                            EmailSubjectConstants.StatusChanged,
                            EmailHtmlMessages.GetStatusChangedHtml(
                                player.Username, sportName, evt.Name, evt.Date, status.GetDisplayName()));
                }
            }
        }

        public bool IsUserJoined(string userId, int eventId) =>
           this.eventsRepository.All()
           .Where(e => e.Id == eventId)
           .Any(e => e.Users.Any(u => u.User.Id == userId));

        public async Task<bool> IsUserAdminOnEventAsync(string userId, int id)
        {
            return await this.GetAsIQuerableById(id)
                .Select(e => e.AdminId)
                .FirstOrDefaultAsync() == userId;
        }

        public async Task<EventInfoViewModel> GetEventByRequestIdAsync(string rentalReqId)
        {
            var evt = await this.eventsRepository.All()
                .Where(e => e.ArenaRentalRequest.Id == rentalReqId)
                .To<EventInfoViewModel>()
                .FirstOrDefaultAsync();

            if (evt == null)
            {
                return null;
            }

            return evt.To<EventInfoViewModel>();
        }

        // Admin
        public async Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId, int? take = null, int skip = 0)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.CountryId == countryId)
                .OrderBy(e => e.CityId)
                .ThenBy(e => e.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>().ToListAsync();
        }

        public async Task<IndexViewModel> AdminFilterAsync(
            int countryId, int? cityId, int? sportId, int? take = null, int skip = 0)
        {
            IQueryable<Event> query = this.eventsRepository.All()
                .Where(e => e.CountryId == countryId)
                .Where(e => e.Status != EventStatus.Passed)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.StartingHour.Hour)
                .ThenBy(e => e.City.Name)
                .ThenBy(e => e.Name);

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

            var resultCount = await query.CountAsync();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var events = await query.To<InfoViewModel>().ToListAsync();

            var countryName = await this.countriesService.GetNameByIdAsync(countryId);
            var location = cityId != null
                ? await this.citiesService.GetNameByIdAsync(cityId.Value) + ", " + countryName
                : countryName;

            var viewModel = new IndexViewModel
            {
                ResultCount = resultCount,
                CountryId = countryId,
                CityId = cityId,
                SportId = sportId,
                Events = events,
                Location = location,
                Filter = new FilterBarViewModel
                {
                    CityId = cityId,
                    SportId = sportId,
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return viewModel;
        }

        public async Task<T> GetEventByIdAsync<T>(int id)
        {
            var query = this.GetAsIQuerableById(id);

            return await query.To<T>().FirstOrDefaultAsync();
        }

        public async Task AdminUpdateAsync(EditViewModel inputModel)
        {
            var evt = await this.GetAsIQuerableById(inputModel.Id).FirstOrDefaultAsync();

            if (evt != null)
            {
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
        }

        public async Task<int> GetCountInCountryAsync(int countryId)
        {
            return await this.eventsRepository.All()
                .Where(e => e.CountryId == countryId)
                .CountAsync();
        }

        private IQueryable<Event> GetAsIQuerableById(int id)
        {
            return this.eventsRepository.All()
                .Where(e => e.Id == id);
        }

        private IQueryable<Event> GetActiveEventsInCountryInPeriodOfTheYearAsIQuerable(
            int countryId, DateTime from, DateTime to)
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

        private async Task ChangeEventStatusAsync(int eventId)
        {
            var evt = await this.GetAsIQuerableById(eventId).FirstOrDefaultAsync();

            if (evt != null)
            {
                var currentStatus = evt.Status;

                if (evt.MaxPlayers == evt.Users.Count)
                {
                    evt.Status = EventStatus.Full;
                }
                else if (evt.MinPlayers > evt.Users.Count)
                {
                    evt.Status = EventStatus.AcceptingPlayers;
                }
                else if (evt.MaxPlayers > evt.Users.Count)
                {
                    evt.Status = EventStatus.MinimumPlayersReached;
                }

                if (currentStatus != evt.Status)
                {
                    this.eventsRepository.Update(evt);
                    await this.eventsRepository.SaveChangesAsync();

                    var sportName = await this.sportsService.GetNameByIdAsync(evt.SportId);
                    var users = await this.eventsUsersRepository.All()
                        .Where(eu => eu.EventId == eventId)
                        .Select(eu => new EmailUserInfo { Email = eu.User.Email, Username = eu.User.UserName, })
                        .ToListAsync();

                    foreach (var user in users)
                    {
                        await this.emailSender.SendEmailAsync(
                            user.Email,
                            EmailSubjectConstants.ChangedStatus,
                            EmailHtmlMessages.GetChangedStatusHtml(
                                user.Username, sportName, evt.Name, evt.Date, currentStatus.GetDisplayName()));
                    }
                }
            }
        }

        private IQueryable<T> GetEventsByArenaAdminIdAsIQueryable<T>(string adminId)
        {
            var query = this.eventsRepository.All()
                .Where(e => e.Arena.ArenaAdminId == adminId);
            query = query
                .Where(e => e.ArenaRentalRequest.Status == ArenaRentalRequestStatus.Approved ||
                            e.ArenaRentalRequest.Status == ArenaRentalRequestStatus.NotApproved)
                .OrderBy(e => e.Date);

            return query.To<T>();
        }

        private async Task<EventDetailsModel> GetEventDetailsForEmailByIdAsync(int eventId)
        {
            var query = this.GetAsIQuerableById(eventId);

            if (query == null)
            {
                return null;
            }

            return await query
                .Select(e => new EventDetailsModel
                {
                    Name = e.Name,
                    Arena = e.Arena.Name,
                    Orginizer = e.Admin.UserName,
                    Date = e.Date,
                    Time = e.StartingHour,
                })
                .FirstOrDefaultAsync();
        }

        private async Task SetPassedStatusAsync(int countryId)
        {
            var eventsToClose = this.eventsRepository
                .All()
                .Where(e => e.Arena.CountryId == countryId)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.Date.AddHours(e.StartingHour.Hour).AddMinutes(e.StartingHour.Minute) < DateTime.UtcNow);

            if (eventsToClose.Any())
            {
                await eventsToClose.ForEachAsync(e => e.Status = EventStatus.Passed);
                await this.eventsRepository.SaveChangesAsync();
            }
        }
    }
}
