namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class ArenaEventsViewModel : IMapFrom<Arena>
    {
        public IEnumerable<ArenaEventsEventInfoViewModel> TodaysEvents { get; set; }

        public IEnumerable<ArenaEventsEventInfoViewModel> ApprovedEvents { get; set; }

        public IEnumerable<ArenaEventsEventInfoViewModel> NotApporvedEvents { get; set; }
    }
}
