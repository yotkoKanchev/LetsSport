namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeIndexLoggedEventsListViewModel
    {
        public IEnumerable<_EventCardPartialViewModel> NotParticipatingEvents { get; set; }

        public _FilterBarPartialViewModel Filter { get; set; }
    }
}
