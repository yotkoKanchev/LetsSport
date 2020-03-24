namespace LetsSport.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EventEditViewModel : IMapFrom<Event>
    {
        public int Id { get; set; }

        public string AdminId { get; set; }

        public string Name { get; set; }

        [DisplayName("Sport")]
        public int SportId { get; set; }

        [DisplayName("Arena")]
        public int ArenaId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DisplayName("Starting Time")]
        [DataType(DataType.Time)]
        public DateTime StartingHour { get; set; }

        [DisplayName("Game Format")]
        public string GameFormat { get; set; }

        public Gender Gender { get; set; }

        [DisplayName("Duration in Hours")]
        public double DurationInHours { get; set; }

        [DisplayName("Minimum Players")]
        public int MinPlayers { get; set; }

        [DisplayName("Maximum Players")]
        public int MaxPlayers { get; set; }

        [DisplayName("Additional Information")]
        public string AdditionalInfo { get; set; }

        public EventStatus Status { get; set; }

        [DisplayName("Request Status")]
        public ArenaRequestStatus RequestStatus { get; set; }

        public IEnumerable<SelectListItem> Arenas { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
