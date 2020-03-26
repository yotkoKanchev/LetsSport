namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    public class ArenaIndexListViewModel
    {
        public IEnumerable<ArenaCardPartialViewModel> Arenas { get; set; }

        public int City { get; set; }

        public int Sport { get; set; }

        public FilterBarArenasPartialViewModel Filter { get; set; }
    }
}
