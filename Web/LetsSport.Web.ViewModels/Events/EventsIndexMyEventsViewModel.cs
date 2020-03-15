namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Home;

    public class EventsIndexMyEventsViewModel
    {
        public IEnumerable<HomeEventInfoViewModel> AdministratingEvents { get; set; }
    }
}
