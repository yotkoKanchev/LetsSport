namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;

    public class EventsIndexMyEventsViewModel
    {
        public IEnumerable<_EventCardPartialViewModel> ParticipatingEvents { get; set; }

        public IEnumerable<_EventCardPartialViewModel> AdministratingEvents { get; set; }

        public IEnumerable<_EventCardPartialViewModel> CanceledEvents { get; set; }
    }
}
