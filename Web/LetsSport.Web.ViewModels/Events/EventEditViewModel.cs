namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class EventEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public string Arena { get; set; }

        public string Date { get; set; }

        public string StartingHour { get; set; }

        public string GameFormat { get; set; }

        public string Gender { get; set; }

        public double DurationInHours { get; set; }

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public string AdditionalInfo { get; set; }

        public string Status { get; set; }

        public string RequestStatus { get; set; }

        public IEnumerable<string> Arenas { get; set; }
    }
}
