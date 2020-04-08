namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    public class ArenaIndexListViewModel
    {
        public IEnumerable<ArenaCardPartialViewModel> Arenas { get; set; }

        public int? CityId { get; set; }

        public int? SportId { get; set; }

        public FilterBarArenasPartialViewModel Filter { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int ResultCount { get; set; }

        public string Location { get; set; }
    }
}
