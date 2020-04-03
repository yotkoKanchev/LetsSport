namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public string Location { get; set; }

        public IEnumerable<InfoViewModel> Arenas { get; set; }

        public FilterBarViewModel Filter { get; set; }

        public int CountryId { get; set; }
    }
}
