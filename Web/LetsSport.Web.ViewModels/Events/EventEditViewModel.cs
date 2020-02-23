namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class EventEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Sport Type")]
        public string SportType { get; set; }

        public string Arena { get; set; }

        public string Date { get; set; }

        [DisplayName("Starting Time")]

        public string StartingHour { get; set; }

        public string GameFormat { get; set; }

        [DisplayName("Game Format")]

        public string Gender { get; set; }

        [DisplayName("Duration in Hours")]
        public double DurationInHours { get; set; }

        [DisplayName("Minimum Players")]

        public int MinPlayers { get; set; }

        [DisplayName("Maximum Players")]
        public int MaxPlayers { get; set; }

        [DisplayName("Additional Information")]
        public string AdditionalInfo { get; set; }

        public string Status { get; set; }

        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }

        public IEnumerable<string> Arenas { get; set; }
    }
}
