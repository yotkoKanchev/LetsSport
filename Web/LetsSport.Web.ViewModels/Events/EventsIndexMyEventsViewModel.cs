namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Home;

    public class EventsIndexMyEventsViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> ParticipatingEvents { get; set; }

        public IEnumerable<HomeEventInfoViewModel> AdministratingEvents { get; set; }

        public IEnumerable<HomeEventInfoViewModel> CanceledEvents { get; set; }
    }
}
