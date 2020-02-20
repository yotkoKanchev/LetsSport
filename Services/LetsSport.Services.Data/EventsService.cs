namespace LetsSport.Services.Data
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.ArenaModels;
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

        public async Task CreateAsync(EventCreateInputModel inputModel)
        {
            var arenaId = this.arenasService.GetArenaId(inputModel.Arena);
            var chatRoomId = await this.chatRoomsService.Create();

            var @event = new Event
            {
                Name = inputModel.Name,
                SportType = (SportType)Enum.Parse(typeof(SportType), inputModel.SportType),
                MinPlayers = inputModel.MinPlayers,
                MaxPlayers = inputModel.MaxPlayers,
                Gender = (Gender)Enum.Parse(typeof(Gender), inputModel.Gender),
                GameFormat = inputModel.GameFormat,
                DurationInHours = inputModel.DurationInHours,
                Date = DateTime.ParseExact(inputModel.Date, "M/d/yyyy hh:mm", CultureInfo.InvariantCulture),
                StartingHour = DateTime.ParseExact(inputModel.StartingHour, "hh:mm", CultureInfo.InvariantCulture),
                AdditionalInfo = inputModel.AdditionalInfo,
                Status = (EventStatus)Enum.Parse(typeof(EventStatus), inputModel.Status),
                RequestStatus = (ArenaRequestStatus)Enum.Parse(typeof(ArenaRequestStatus), inputModel.RequestStatus),
                ArenaId = arenaId,
                ChatRoomId = chatRoomId,
                CreatedOn = DateTime.UtcNow,
            };

            await this.db.Events.AddAsync(@event);
            await this.db.SaveChangesAsync();
        }
    }
}
