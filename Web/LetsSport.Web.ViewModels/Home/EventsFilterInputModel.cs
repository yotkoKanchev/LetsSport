namespace LetsSport.Web.ViewModels.Home
{
    using System;

    public class EventsFilterInputModel 
    {
        public string City { get; set; }

        //public string Country { get; set; }

        public string Sport { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}
