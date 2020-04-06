namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using System.Collections.Generic;

    public class IndexListViewModel
    {
        public IEnumerable<InfoViewModel> Countries { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public int CountryId { get; set; }
    }
}
