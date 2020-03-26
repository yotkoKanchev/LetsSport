namespace LetsSport.Services.Models
{
    using System;

    public class EventDetailsModel
    {
        public string Name { get; set; }

        public string Sport { get; set; }

        public string Arena { get; set; }

        public string Orginizer { get; set; }

        public DateTime Date { get; set; }

        public DateTime Time { get; set; }
    }
}
