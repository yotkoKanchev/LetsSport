namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeIndexLoggedEventsListViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> NotParticipatingEvents { get; set; }

        public _FilterBarPartialViewModel Partial { get; set; }
    }
}
