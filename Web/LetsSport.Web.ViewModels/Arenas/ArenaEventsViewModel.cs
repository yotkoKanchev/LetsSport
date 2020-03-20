namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class ArenaEventsViewModel : IMapFrom<Arena>
    {
        public IEnumerable<ArenaEventsEventInfoViewModel> Events { get; set; }
    }
}
