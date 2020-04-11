namespace LetsSport.Web.ViewModels.Home
{
    using System;
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeEventsListViewModel
    {
        public IEnumerable<EventCardPartialViewModel> Events { get; set; }

        public FilterBarPartialViewModel Filter { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public string Location { get; set; }

        public int? CityId { get; set; }

        public int? SportId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int ResultCount { get; set; }
    }
}
