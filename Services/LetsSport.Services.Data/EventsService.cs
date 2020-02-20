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
    using Microsoft.AspNetCore.Identity;

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
                AdminId = userId,
            };

            await this.db.Events.AddAsync(@event);
            await this.db.SaveChangesAsync();
        }
    }
}
