namespace LetsSport.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Common;
    using LetsSport.Data.Common;
    using LetsSport.Data.Models.Events;
    using LetsSport.Data.Models.Users;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.EventsUsers;

    public class EventDetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportName { get; set; }

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

        [DisplayName("Arena Request")]
        public string RequestStatus { get; set; }

        public string Status { get; set; }

        public string AdminUserName { get; set; }

        public string AdminId { get; set; }

        public string AdminScore { get; set; }

        // custom props
        [DisplayName("Total price")]
        public double TotalPrice { get; set; }

        [DisplayName("Empty spots")]
        public int EmptySpotsLeft { get; set; }

        [DisplayName("Request Deadline")]
        public string DeadLineToSendRequest { get; set; }

        public IEnumerable<EventUserViewModel> ChatRoomUsers { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventDetailsViewModel>()
                 .ForMember(vm => vm.DeadLineToSendRequest, opt => opt.MapFrom(e => e.Date.AddDays(-2).ToString(GlobalConstants.DefaultDateFormat)))
                 .ForMember(vm => vm.EmptySpotsLeft, opt => opt.MapFrom(e => e.MaxPlayers - e.Users.Count))
                 .ForMember(vm => vm.TotalPrice, opt => opt.MapFrom(e => Math.Round(e.Arena.PricePerHour * e.DurationInHours, 2)))
                 .ForMember(vm => vm.AdminScore, opt => opt.MapFrom(e => $"{e.Admin.Events.Count}/{e.Admin.AdministratingEvents.Count}"))
                 .ForMember(x => x.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus.GetDisplayName()))
                 .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status.GetDisplayName()));
        }
    }
}
