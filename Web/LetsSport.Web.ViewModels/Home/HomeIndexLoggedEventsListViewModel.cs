namespace LetsSport.Web.ViewModels.Home
{
    using System.Collections.Generic;

    public class HomeIndexLoggedEventsListViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> ParticipatingEvents { get; set; }

        public IEnumerable<HomeEventInfoViewModel> NotParticipatingEvents { get; set; }
    }
}
