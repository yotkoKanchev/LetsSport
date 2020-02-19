namespace LetsSport.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    public class EventCreateInputModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string SportType { get; set; }

        public string Arena { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string StartingHour { get; set; }

        public int DurationInHours { get; set; }

        [MaxLength(50)]
        public string GameFormat { get; set; }

        [Range(2, 30)]
        public int MinPlayers { get; set; }

        [Range(2, 30)]
        public int MaxPlayers { get; set; }

        public string Gender { get; set; }

        [MaxLength(1000)]
        public string AdditionalInfo { get; set; }

        public string Status { get; set; }

        public string RequestStatus { get; set; }
    }
}
