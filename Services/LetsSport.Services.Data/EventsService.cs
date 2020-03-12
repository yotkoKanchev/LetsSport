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
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Data.Common;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;
    using LetsSport.Web.ViewModels.Messages;

    public class EventsService : IEventsService
    {
        private readonly IArenasService arenasService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Event> eventsRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly ICitiesService citiesService;
        private readonly SportImageUrl sportImages;

        public EventsService(
            IArenasService arenasService,
            ISportsService sportsService,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            ICitiesService citiesService)
        {
            this.arenasService = arenasService;
            this.sportsService = sportsService;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.citiesService = citiesService;
            this.sportImages = new SportImageUrl();
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId)
        {
            var @event = new Event
            {
                Name = inputModel.Name,
                SportId = inputModel.Sport,
                MinPlayers = inputModel.MinPlayers,
                MaxPlayers = inputModel.MaxPlayers,
                Gender = inputModel.Gender,
                GameFormat = inputModel.GameFormat,
                DurationInHours = inputModel.DurationInHours,
                Date = inputModel.Date,
                StartingHour = inputModel.StartingHour,
                AdditionalInfo = inputModel.AdditionalInfo,
                Status = inputModel.Status,
                RequestStatus = inputModel.RequestStatus,
                ArenaId = inputModel.Arena,
                AdminId = userId,
            };

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

        public EventEditViewModel GetDetailsForEdit(int id, string city, string country)
        {
            var viewModel = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EventEditViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Arena = e.Arena.Name + ", " + e.Arena.Address.City.Name + ", " + e.Arena.Address.City.Country.Name,
                    Sport = e.Sport.Name,
                    Gender = e.Gender.ToString(),
                    GameFormat = e.GameFormat,
                    Date = e.Date.ToString("dd.MM.yyyy"),
                    StartingHour = e.StartingHour.ToString("hh:mm"),
                    DurationInHours = e.DurationInHours,
                    MaxPlayers = e.MaxPlayers,
                    MinPlayers = e.MinPlayers,
                    AdditionalInfo = e.AdditionalInfo,
                    Status = e.Status.ToString(),
                    RequestStatus = e.RequestStatus.ToString(),
                })
                .FirstOrDefault();

            var arenas = this.arenasService.GetArenas(city, country);
            viewModel.Arenas = null;

            return viewModel;
        }

        public EventDetailsViewModel GetEvent(int id)
        {
            var inputModel = this.eventsRepository
                .All()
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Arena = e.Arena.Name,
                    Sport = e.Sport.Name,
                    Date = e.Date.ToString("dd.MM.yyyy"),
                    Gender = e.Gender.ToString(),
                    GameFormat = e.GameFormat,
                    DurationInHours = e.DurationInHours,
                    AdditionalInfo = e.AdditionalInfo,
                    MaxPlayers = e.MaxPlayers,
                    MinPlayers = e.MinPlayers,
                    StartingHour = e.StartingHour.ToString("hh:mm"),
                    RequestStatus = e.RequestStatus.ToString(),
                    Status = e.Status.ToString(),
                    Admin = e.Admin.UserName,
                    UserProfileId = e.Admin.UserProfile.Id,
                    TotalPrice = e.Arena.PricePerHour * e.DurationInHours,
                    DeadLineToSendRequest = e.Date.AddDays(-2).ToString("dd.MM.yyyy"),
                    EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                    NeededPlayersForConfirmation = e.MinPlayers > e.Users.Count ? e.MinPlayers - e.Users.Count : 0,
                    Players = string.Join(", ", e.Users
                            .Select(s => s.User.UserName)
                            .ToList()),
                    Messages = e.Messages
                            .OrderByDescending(m => m.CreatedOn)
                            .Select(m => new MessageDetailsViewModel
                            {
                                CreatedOn = m.CreatedOn.ToString("dd-MM-yyy hh:mm"),
                                Sender = m.Sender.UserName,
                                Text = m.Content,
                            }).ToList(),
                })
                .FirstOrDefault();

            return inputModel;
        }

        public async Task UpdateEvent(EventEditViewModel viewModel)
        {
            var hours = TimeSpan.Parse(viewModel.StartingHour);

            var updatedEvent = this.eventsRepository
                .AllAsNoTracking()
                .First(e => e.Id == viewModel.Id);

            updatedEvent.Name = viewModel.Name;
            updatedEvent.MinPlayers = viewModel.MinPlayers;
            updatedEvent.MaxPlayers = viewModel.MaxPlayers;
            updatedEvent.Gender = viewModel.Gender != null
                ? (Gender)Enum.Parse(typeof(Gender), viewModel.Gender)
                : updatedEvent.Gender;
            updatedEvent.GameFormat = viewModel.GameFormat;
            updatedEvent.DurationInHours = viewModel.DurationInHours;
            updatedEvent.Date = viewModel.Date != null ? Convert.ToDateTime(viewModel.Date) : updatedEvent.Date;
            updatedEvent.StartingHour = viewModel.StartingHour != null
                ? updatedEvent.Date.AddHours(hours.Hours)
                : updatedEvent.StartingHour;
            updatedEvent.AdditionalInfo = viewModel.AdditionalInfo;
            updatedEvent.Status = viewModel.Status != null
                ? (EventStatus)Enum.Parse(typeof(EventStatus), viewModel.Status)
                : updatedEvent.Status;
            updatedEvent.RequestStatus = viewModel.RequestStatus != null
                ? (ArenaRequestStatus)Enum.Parse(typeof(ArenaRequestStatus), viewModel.RequestStatus)
                : updatedEvent.RequestStatus;

            this.eventsRepository.Update(updatedEvent);
            await this.eventsRepository.SaveChangesAsync();
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

        public async Task<HomeEventsListViewModel> FilterEventsAsync(EventsFilterInputModel inputModel, string currentCity, string currentCountry)
        {
            // get Querry result and add all filters on it. After that mapp it to the model !!!
            var startDate = DateTime.UtcNow;
            if (inputModel.From != null)
            {
                startDate = DateTime.Parse(inputModel.From);
            }

            var endDate = DateTime.UtcNow.AddMonths(6);
            if (inputModel.To != null)
            {
                endDate = DateTime.Parse(inputModel.To);
            }

            var cityName = currentCity;
            if (inputModel.City != null && inputModel.City != cityName)
            {
                cityName = inputModel.City;
            }

            var cities = await this.citiesService.GetCitiesWhitEventsAsync(currentCity, currentCountry);

            if (inputModel.Sport != null)
            {
                var sportId = this.sportsService.GetSportId(inputModel.Sport);
                var sports = this.GetAllSportsInCurrentCountry(currentCountry);

                var viewModel = new HomeEventsListViewModel()
                {
                    Events = this.eventsRepository
                    .All()
                    .Where(e => e.Arena.Address.City.Name == cityName &&
                                e.Arena.Address.City.Country.Name == currentCountry)
                    .Where(e => e.Status != EventStatus.Passed)
                    .Where(e => e.Date >= startDate &&
                                e.Date <= endDate &&
                                e.SportId == 1/*sportType FIND SPORT ID*/)
                    .Where(e => e.MaxPlayers > e.Users.Count)
                    .OrderBy(e => e.Date)
                    .Select(e => new HomeEventInfoViewModel
                    {
                        Id = e.Id,
                        ArenaName = e.Arena.Name,
                        SportName = e.Sport.Name,
                        Date = e.Date.ToString("dd-MMM-yyyy") + " at " + e.StartingHour.ToString("hh:mm"),
                        EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                        SportImage = e.Sport.Image,
                    })
                    .ToList(),
                    Cities = cities,
                    Sports = sports,
                };

                return viewModel;
            }
            else
            {
                var sports = this.GetAllSportsByCityName(cityName, currentCountry);

                var viewModel = new HomeEventsListViewModel()
                {
                    Events = this.eventsRepository
                   .All()
                   .Where(e => e.Arena.Address.City.Name == cityName &&
                               e.Arena.Address.City.Country.Name == currentCountry)
                   .Where(e => e.Status != EventStatus.Passed)
                   .Where(e => e.Date >= startDate && e.Date <= endDate)
                   .OrderBy(e => e.Date)
                   .Select(e => new HomeEventInfoViewModel
                   {
                       Id = e.Id,
                       ArenaName = e.Arena.Name,
                       SportName = e.Sport.Name,
                       Date = e.Date.ToString("dd-MMM-yyyy") + " at " + e.StartingHour.ToString("hh:mm"),
                       EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                       SportImage = e.Sport.Image,
                   })
                   .ToList(),
                    Cities = cities,
                    Sports = sports,
                };

                return viewModel;
            }
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

        private HashSet<string> GetAllSportsByCityName(string cityName, string currentCountry)
        {
            var sports = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry &&
                            e.Arena.Address.City.Name == cityName)
                .Select(e => e.Sport.Name)
                .ToHashSet();

            return sports;
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
