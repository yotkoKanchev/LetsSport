namespace LetsSport.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.EventsUsers;
    using LetsSport.Web.ViewModels.Messages;

    public class EventDetailsViewModel : IMapFrom<Event>, IMapTo<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Sport { get; set; }

        public string SportImage { get; set; }

        public int ArenaId { get; set; }

        public string ArenaName { get; set; }

        public DateTime Date { get; set; }

        [DisplayName("Start time")]
        public DateTime StartingHour { get; set; }

        [DisplayName("Game format")]
        public string GameFormat { get; set; }

        [DisplayName("Duration in hours")]
        public double DurationInHours { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Minimum players")]
        public int MinPlayers { get; set; }

        [DisplayName("Maximum players")]
        public int MaxPlayers { get; set; }

        [DisplayName("Additional info")]
        public string AdditionalInfo { get; set; }

        [DisplayName("Request Status")]
        public ArenaRequestStatus RequestStatus { get; set; }

        public EventStatus Status { get; set; }

        public string AdminUserName { get; set; }

        public string AdminId { get; set; }

        // custom props
        [DisplayName("Total price")]
        public double TotalPrice { get; set; }

        [DisplayName("Empty spots")]
        public int EmptySpotsLeft { get; set; }

        [DisplayName("Needed Players")]
        public int NeededPlayersForConfirmation { get; set; }

        [DisplayName("Request Deadline")]
        public string DeadLineToSendRequest { get; set; }

        // chatroom props
        public IEnumerable<EventUserViewModel> ChatRoomUsers { get; set; }

        public string MessageContent { get; set; }

        public IEnumerable<MessageDetailsViewModel> ChatRoomMessages { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventDetailsViewModel>()
                 .ForMember(vm => vm.DeadLineToSendRequest, opt => opt.MapFrom(e => e.Date.AddDays(-2).ToString("dd.MM.yyyy")))
                 .ForMember(vm => vm.EmptySpotsLeft, opt => opt.MapFrom(e => e.MinPlayers - e.Users.Count))
                 .ForMember(vm => vm.NeededPlayersForConfirmation, opt => opt.MapFrom(e => e.MinPlayers > e.Users.Count ? e.MinPlayers - e.Users.Count : 0))
                 .ForMember(vm => vm.TotalPrice, opt => opt.MapFrom(e => Math.Round(e.Arena.PricePerHour * e.DurationInHours, 2)));
        }
    }
}
