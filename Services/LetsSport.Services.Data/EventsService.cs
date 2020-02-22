namespace LetsSport.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Web.ViewModels.Events;

    public class EventsService : IEventsService
    {
        private readonly IArenasService arenasService;
        private readonly ApplicationDbContext db;
        private readonly IChatRoomsService chatRoomsService;

        public EventsService(IArenasService arenasService, ApplicationDbContext db, IChatRoomsService chatRoomsService)
        {
            this.arenasService = arenasService;
            this.db = db;
            this.chatRoomsService = chatRoomsService;
        }

        public async Task CreateAsync(EventCreateInputModel inputModel, string userId)
        {
            var arenaId = this.arenasService.GetArenaId(inputModel.Arena);
            var chatRoomId = await this.chatRoomsService.Create();
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
                ChatRoomId = chatRoomId,
                CreatedOn = DateTime.UtcNow,
                AdminId = /*userId*/ "568513a1-5ce1-4e19-a76c-6ca9eaeb2594",
            };

            await this.db.Events.AddAsync(@event);
            await this.db.SaveChangesAsync();
        }

        public EventEditViewModel GetDetailsForEdit(int id)
        {
            var viewModel = this.db.Events
                .Where(e => e.Id == id)
                .Select(e => new EventEditViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Arena = e.Arena.Name + ", " + e.Arena.Address.City.Name + ", " + e.Arena.Address.City.Country.Name,
                    SportType = e.SportType.ToString(),
                    Gender = e.Gender.ToString(),
                    GameFormat = e.GameFormat,
                    Date = e.Date.ToString("R"),
                    StartingHour = e.Date.ToString("hh:mm"),
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
            var inputModel = this.db.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Arena = e.Arena.Name,
                    SportType = e.SportType.ToString(),
                    Date = e.Date.ToString("R"),
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
                    DeadLineToSendRequest = e.Date.AddDays(-2).ToString("R"),
                    EmptySpotsLeft = e.MaxPlayers - e.Sporters.Count,
                    NeededPlayersForConfirmation = e.MinPlayers - e.Sporters.Count,
                    Players = string.Join(", ", e.Sporters
                            .Select(s => s.User.UserName)
                            .ToList()),
                })
                .FirstOrDefault();

            return inputModel;
        }

        public void UpdateEvent(EventEditViewModel viewModel)
        {
            var hours = TimeSpan.Parse(viewModel.StartingHour);

            var @event = this.db.Events.Find(viewModel.Id);

            @event.Name = viewModel.Name;
            @event.MinPlayers = viewModel.MinPlayers;
            @event.MaxPlayers = viewModel.MaxPlayers;
            //@event.Gender = (Gender)Enum.Parse(typeof(Gender), viewModel.Gender);
            @event.GameFormat = viewModel.GameFormat;
            @event.DurationInHours = viewModel.DurationInHours;
            @event.Date = Convert.ToDateTime(viewModel.Date);
            @event.StartingHour = @event.Date.AddHours(hours.Hours);
            @event.AdditionalInfo = viewModel.AdditionalInfo;
            //@event.Status = (EventStatus)Enum.Parse(typeof(EventStatus), viewModel.Status);
            //@event.RequestStatus = (ArenaRequestStatus)Enum.Parse(typeof(ArenaRequestStatus), viewModel.RequestStatus);
            //@event.ArenaId = this.arenasService.GetArenaId(viewModel.Arena);

            this.db.Events.Update(@event);
            this.db.SaveChanges();
        }
    }
}
