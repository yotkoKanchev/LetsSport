namespace LetsSport.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    public class EventCreateInputModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sport Type")]
        public string SportType { get; set; }

        public string Arena { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Starting Time")]
        public string StartingHour { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Duration in hours")]
        public int DurationInHours { get; set; }

        [MaxLength(50)]
        [Display(Name = "Game Format")]
        public string GameFormat { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Maximum Players")]
        public int MinPlayers { get; set; }

        [Display(Name = "Minimum Players")]
        [Range(0, int.MaxValue, ErrorMessage ="Minimum number of players can not be less than 1!")]
        public int MaxPlayers { get; set; }

        public string Gender { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Addtional Information")]
        public string AdditionalInfo { get; set; }

        public string Status { get; set; }

        [Display(Name = "Request Status")]
        public string RequestStatus { get; set; }
    }
}
