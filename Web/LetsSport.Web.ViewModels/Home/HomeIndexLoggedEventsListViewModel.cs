namespace LetsSport.Web.ViewModels.Home
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class HomeIndexLoggedEventsListViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> NotParticipatingEvents { get; set; }

        public IEnumerable<string> Cities { get; set; }

        public IEnumerable<string> Sports { get; set; }

        public string Sport { get; set; }

        public string City { get; set; }

        [DataType(DataType.Date)]
        public DateTime From { get; set; }

        [DataType(DataType.Date)]
        public DateTime To { get; set; }
    }
}
