namespace LetsSport.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EventsListViewModel
    {
        public IEnumerable<EventInfoViewModel> Events { get; set; }

        public IEnumerable<string> Cities { get; set; }

        public IEnumerable<string> Sports { get; set; }

        public string Sport { get; set; }

        [DataType(DataType.Date)]
        public DateTime? From { get; set; }

        [DataType(DataType.Date)]
        public DateTime? To { get; set; }
    }
}
