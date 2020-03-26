namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeIndexLoggedEventsListViewModel
    {
        public IEnumerable<EventCardPartialViewModel> NotParticipatingEvents { get; set; }

        public FilterBarPartialViewModel Filter { get; set; }
    }
}
