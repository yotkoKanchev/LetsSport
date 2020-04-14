namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using System.Collections.Generic;

    public class IndexListViewModel
    {
        public IEnumerable<InfoViewModel> Countries { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }
    }
}
