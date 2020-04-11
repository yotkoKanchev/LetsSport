namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    public class ArenaEventsViewModel
    {
        public IEnumerable<ArenaEventsEventInfoViewModel> TodaysEvents { get; set; }

        public IEnumerable<ArenaEventsEventInfoViewModel> ApprovedEvents { get; set; }

        public IEnumerable<ArenaEventsEventInfoViewModel> NotApporvedEvents { get; set; }
    }
}
