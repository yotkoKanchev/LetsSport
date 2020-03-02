namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Web.ViewModels.Events;
    using LetsSport.Web.ViewModels.Home;

    public class EventsService : IEventsService
    {
        private readonly IArenasService arenasService;
        private readonly IRepository<Event> eventsRepository;
        private readonly IRepository<EventUser> eventsUsersRepository;
        private readonly IChatRoomsService chatRoomsService;
        private readonly SportImageUrl sportImages;

        public EventsService(
            IArenasService arenasService,
            IRepository<Event> eventsRepository,
            IRepository<EventUser> eventsUsersRepository,
            IChatRoomsService chatRoomsService)
        {
            this.arenasService = arenasService;
            this.eventsRepository = eventsRepository;
            this.eventsUsersRepository = eventsUsersRepository;
            this.chatRoomsService = chatRoomsService;
            this.sportImages = new SportImageUrl();
        }

        public async Task<int> CreateAsync(EventCreateInputModel inputModel, string userId)
        {
            var arenaId = this.arenasService.GetArenaId(inputModel.Arena);
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

            await this.eventsRepository.SaveChangesAsync();

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

        public EventsAllDetailsViewModel GetAll()
        {
            var viewModel = new EventsAllDetailsViewModel()
            {
                AllEvents = this.eventsRepository
                .AllAsNoTracking()
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
            };

            return viewModel;
        }

        public EventEditViewModel GetDetailsForEdit(int id)
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

            var arenas = this.arenasService.GetArenas();
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
                    NeededPlayersForConfirmation = e.MinPlayers - e.Users.Count,
                    Players = string.Join(", ", e.Users
                            .Select(s => s.User.UserName)
                            .ToList()),
                })
                .FirstOrDefault();

            return inputModel;
        }

        public void UpdateEvent(EventEditViewModel viewModel)
        {
            var hours = TimeSpan.Parse(viewModel.StartingHour);

            var @event = this.eventsRepository
                .AllAsNoTracking()
                .First(e => e.Id == viewModel.Id);

            @event.Name = viewModel.Name;
            @event.MinPlayers = viewModel.MinPlayers;
            @event.MaxPlayers = viewModel.MaxPlayers;
            @event.Gender = viewModel.Gender != null ? (Gender)Enum.Parse(typeof(Gender), viewModel.Gender) : @event.Gender;
            @event.GameFormat = viewModel.GameFormat;
            @event.DurationInHours = viewModel.DurationInHours;
            @event.Date = viewModel.Date != null ? Convert.ToDateTime(viewModel.Date) : @event.Date;
            @event.StartingHour = viewModel.StartingHour != null ? @event.Date.AddHours(hours.Hours) : @event.StartingHour;
            @event.AdditionalInfo = viewModel.AdditionalInfo;
            @event.Status = viewModel.Status != null ? (EventStatus)Enum.Parse(typeof(EventStatus), viewModel.Status) : @event.Status;
            @event.RequestStatus = viewModel.RequestStatus != null
                ? (ArenaRequestStatus)Enum.Parse(typeof(ArenaRequestStatus), viewModel.RequestStatus)
                : @event.RequestStatus;

            this.eventsRepository.Update(@event);
            this.eventsRepository.SaveChangesAsync();
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
        }

        public async Task RemoveUserAsync(int eventId, string userId)
        {
            var eventUser = this.eventsUsersRepository.All()
                .Where(eu => eu.EventId == eventId && eu.UserId == userId)
                .FirstOrDefault();

            this.eventsUsersRepository.Delete(eventUser);
            await this.eventsUsersRepository.SaveChangesAsync();
        }

        public EventsAllDetailsViewModel FilterEventsAsync(EventsFilterInputModel inputModel)
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

            if (inputModel.Sport != null)
            {
                var sportType = (SportType)Enum.Parse<SportType>(inputModel.Sport);
                var viewModel = new EventsAllDetailsViewModel()
                {
                    AllEvents = this.eventsRepository
                    .All()
                    .Where(e => e.Date >= startDate && e.Date <= endDate && e.SportType == sportType)
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
                };

                return viewModel;
            }
            else
            {
                var viewModel = new EventsAllDetailsViewModel()
                {
                    AllEvents = this.eventsRepository
                   .All()
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
                };

                return viewModel;
            }
        }
    }
}
