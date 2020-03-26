namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class EventsIndexMyEventsViewModel
    {
        public IEnumerable<EventCardPartialViewModel> ParticipatingEvents { get; set; }

        public IEnumerable<EventCardPartialViewModel> AdministratingEvents { get; set; }

        public IEnumerable<EventCardPartialViewModel> CanceledEvents { get; set; }
    }
}
