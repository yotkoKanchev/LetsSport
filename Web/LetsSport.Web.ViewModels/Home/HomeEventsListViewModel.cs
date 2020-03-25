namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class HomeEventsListViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> Events { get; set; }

        public _FilterBarPartialViewModel Partial { get; set; }
    }
}
