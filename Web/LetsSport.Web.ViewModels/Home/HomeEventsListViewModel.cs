namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeEventsListViewModel
    {
        public IEnumerable<_EventCardPartialViewModel> Events { get; set; }

        public _FilterBarPartialViewModel Filter { get; set; }
    }
}
