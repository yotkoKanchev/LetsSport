namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;
    using System.Linq;

    public class IndexViewModel
    {
        public IEnumerable<InfoViewModel> Cities { get; set; }

        public string Location { get; set; }

        public FilterBarViewModel Filter { get; set; }

        public int CountryId { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public int ResultsCount { get; set; }
    }
}
