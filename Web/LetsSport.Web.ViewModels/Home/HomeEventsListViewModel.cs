namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeEventsListViewModel
    {
        public IEnumerable<EventCardPartialViewModel> Events { get; set; }

        public FilterBarPartialViewModel Filter { get; set; }
    }
}
