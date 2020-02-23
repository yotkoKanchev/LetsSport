namespace LetsSport.Web.ViewModels.Events
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class EventCreateInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [DisplayName("Sport Type")]
        [Required]
        public string SportType { get; set; }

        public string Arena { get; set; }

        [Required]
        public string Date { get; set; }

        [DisplayName("Starting Time")]
        [Required]
        public string StartingHour { get; set; }

        public int DurationInHours { get; set; }

        [DisplayName("Game Format")]
        [MaxLength(50)]
        public string GameFormat { get; set; }

        [DisplayName("Maximum Players")]
        [Range(2, 30)]
        public int MinPlayers { get; set; }

        [DisplayName("Minimum Players")]
        [Range(2, 30)]
        public int MaxPlayers { get; set; }

        public string Gender { get; set; }

        [DisplayName("Addtional Information")]
        [MaxLength(1000)]
        public string AdditionalInfo { get; set; }

        public string Status { get; set; }

        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }
    }
}
