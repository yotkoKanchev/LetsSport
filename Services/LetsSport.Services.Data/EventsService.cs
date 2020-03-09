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
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public class EventsService : IEventsService
    {
        private readonly IArenasService arenasService;
        private readonly IRepository<Event> eventsRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly IChatRoomsService chatRoomsService;
        private readonly ICitiesService citiesService;
        private readonly SportImageUrl sportImages;

        public EventsService(
            IArenasService arenasService,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            IChatRoomsService chatRoomsService,
            ICitiesService citiesService)
        {
            this.arenasService = arenasService;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.chatRoomsService = chatRoomsService;
            this.citiesService = citiesService;
            this.sportImages = new SportImageUrl();
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId, string city, string country)
        {
            var arenaId = this.arenasService.GetArenaId(inputModel.Arena, city, country);
            var dateAsDateTime = Convert.ToDateTime(inputModel.Date);
            var startTimeAsTimeSpan = TimeSpan.Parse(inputModel.StartingHour);

            var @event = new Event
            {
                Name = inputModel.Name,
                SportType = (SportType)Enum.Parse(typeof(SportType), inputModel.SportType),
                MinPlayers = inputModel.MinPlayers,
                MaxPlayers = inputModel.MaxPlayers,
                Gender = (Gender)Enum.Parse(typeof(Gender), inputModel.Gender),
                GameFormat = inputModel.GameFormat,
                DurationInHours = inputModel.DurationInHours,
                Date = dateAsDateTime,
                StartingHour = dateAsDateTime.AddHours(startTimeAsTimeSpan.Hours),
                AdditionalInfo = inputModel.AdditionalInfo,
                Status = (EventStatus)Enum.Parse(typeof(EventStatus), inputModel.Status),
                RequestStatus = (ArenaRequestStatus)Enum.Parse(typeof(ArenaRequestStatus), inputModel.RequestStatus),
                ArenaId = arenaId,
                CreatedOn = DateTime.UtcNow,
                AdminId = userId,
            };

            await this.eventsRepository.AddAsync(@event);
            await this.eventsRepository.SaveChangesAsync();
            await this.chatRoomsService.CreateAsync(@event.Id, userId);

            await this.eventsUsersRepository.AddAsync(new EventUser
            {
                EventId = @event.Id,
                UserId = userId,
            });

            await this.eventsUsersRepository.SaveChangesAsync();

            return @event.Id;
        }

        public int GetIdByChatRoomId(string chatRoomId)
        {
            return this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.ChatRoom.Id == chatRoomId)
                .Select(e => e.Id)
                .FirstOrDefault();
        }

        public async Task<EventsAllDetailsViewModel> GetAll(string currentCity, string currentCountry)
        {
            await this.SetPassedStatusOnPassedEvents(currentCity, currentCountry);
            var cities = await this.citiesService.GetCitiesWhitEventsAsync(currentCity, currentCountry);
            var sports = this.GetAllSportsInCurrentCountry(currentCountry);

            var viewModel = new EventsAllDetailsViewModel()
            {
                AllEvents = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Status != EventStatus.Passed &&
                            e.Status != EventStatus.Full)
                .Where(e => e.MaxPlayers > e.Users.Count)
                .OrderBy(e => e.Date)
                .Select(e => new EventInfoViewModel
                {
                    Id = e.Id,
                    Arena = e.Arena.Name,
                    Sport = e.SportType.ToString(),
                    Date = e.Date.ToString("dd-MMM-yyyy") + " at " + e.StartingHour.ToString("hh:mm"),
                    EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                    ImgUrl = this.sportImages.GetSportPath(e.SportType.ToString()),
                })
                .ToList(),
                Cities = cities,
                Sports = sports,
            };

            return viewModel;
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
                    SportType = e.SportType.ToString(),
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
            viewModel.Arenas = arenas;

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
                    SportType = e.SportType.ToString(),
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
                    TotalPrice = e.Arena.PricePerHour * e.DurationInHours,
                    DeadLineToSendRequest = e.Date.AddDays(-2).ToString("dd.MM.yyyy"),
                    EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                    NeededPlayersForConfirmation = e.MinPlayers > e.Users.Count ? e.MinPlayers - e.Users.Count : 0,
                    Players = string.Join(", ", e.Users
                            .Select(s => s.User.UserName)
                            .ToList()),
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

        public async Task<EventsAllDetailsViewModel> FilterEventsAsync(EventsFilterInputModel inputModel, string currentCity, string currentCountry)
        {
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
                var sportType = (SportType)Enum.Parse<SportType>(inputModel.Sport);
                var sports = this.GetAllSportsInCurrentCountry(currentCountry);
                var viewModel = new EventsAllDetailsViewModel()
                {
                    AllEvents = this.eventsRepository
                    .All()
                    .Where(e => e.Arena.Address.City.Name == cityName &&
                                e.Arena.Address.City.Country.Name == currentCountry)
                    .Where(e => e.Status != EventStatus.Passed)
                    .Where(e => e.Date >= startDate &&
                                e.Date <= endDate &&
                                e.SportType == sportType)
                    .Where(e => e.MaxPlayers > e.Users.Count)
                    .OrderBy(e => e.Date)
                    .Select(e => new EventInfoViewModel
                    {
                        Id = e.Id,
                        Arena = e.Arena.Name,
                        Sport = e.SportType.ToString(),
                        Date = e.Date.ToString("dd-MMM-yyyy") + " at " + e.StartingHour.ToString("hh:mm"),
                        EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                        ImgUrl = this.sportImages.GetSportPath(e.SportType.ToString()),
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

                var viewModel = new EventsAllDetailsViewModel()
                {
                    AllEvents = this.eventsRepository
                   .All()
                   .Where(e => e.Arena.Address.City.Name == cityName &&
                               e.Arena.Address.City.Country.Name == currentCountry)
                   .Where(e => e.Status != EventStatus.Passed)
                   .Where(e => e.Date >= startDate && e.Date <= endDate)
                   .OrderBy(e => e.Date)
                   .Select(e => new EventInfoViewModel
                   {
                       Id = e.Id,
                       Arena = e.Arena.Name,
                       Sport = e.SportType.ToString(),
                       Date = e.Date.ToString("dd-MMM-yyyy") + " at " + e.StartingHour.ToString("hh:mm"),
                       EmptySpotsLeft = e.MaxPlayers - e.Users.Count,
                       ImgUrl = this.sportImages.GetSportPath(e.SportType.ToString()),
                   })
                   .ToList(),
                    Cities = cities,
                    Sports = sports,
                };

                return viewModel;
            }
        }

        private async Task SetPassedStatusOnPassedEvents(string currentCity, string currentCountry)
        {
            var eventsToClose = this.eventsRepository
                .All()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Where(e => e.Arena.Address.City.Name == currentCity)
                .Where(e => e.Status != EventStatus.Passed)
                .Where(e => e.Date <= DateTime.UtcNow.AddHours(-1));

            foreach (var @event in eventsToClose)
            {
                @event.Status = EventStatus.Passed;
            }

            await this.eventsRepository.SaveChangesAsync();
        }

        private HashSet<string> GetAllSportsInCurrentCountry(string currentCountry)
        {
            var sports = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry)
                .Select(e => e.SportType.ToString())
                .ToHashSet();

            return sports;
        }

        private HashSet<string> GetAllSportsByCityName(string cityName, string currentCountry)
        {
            var sports = this.eventsRepository
                .AllAsNoTracking()
                .Where(e => e.Arena.Address.City.Country.Name == currentCountry &&
                            e.Arena.Address.City.Name == cityName)
                .Select(e => e.SportType.ToString())
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
