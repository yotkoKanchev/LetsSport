namespace LetsSport.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EventCreateInputModel : IMapTo<Event>
    {
        public string AdminId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sport")]
        public int SportId { get; set; }

        [Display(Name = "Arena")]
        public int ArenaId { get; set; }

        public int CityId { get; set; }

        public int CountryId { get; set; }

        // TODO Add attribute to check if it only future date
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // TODO Add attribute to check if it only future date
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Starting Time")]
        public DateTime StartingHour { get; set; }

        [Range(0, 24)]
        [Display(Name = "Duration in hours")]
        public int DurationInHours { get; set; }

        [MaxLength(50)]
        [Display(Name = "Game Format")]
        public string GameFormat { get; set; }

        [Range(0, 10000, ErrorMessage = "Maximum number of players can not be less than 0 and more than 10000!")]
        [Display(Name = "Maximum Players")]
        public int MaxPlayers { get; set; }

        // TODO Add attribute to check if it is less than max players
        [Display(Name = "Minimum Players")]
        [Range(0, 10000, ErrorMessage = "Minimum number of players can not be less than 0 and more than 10000!")]
        public int MinPlayers { get; set; }

        public Gender Gender { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Addtional Information")]
        public string AdditionalInfo { get; set; }

        public EventStatus Status { get; set; }

        [Display(Name = "Arena Request Status")]
        public ArenaRequestStatus RequestStatus { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        public IEnumerable<SelectListItem> Arenas { get; set; }
    }
}
