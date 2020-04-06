namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using System.Collections.Generic;

    public class IndexListViewModel
    {
        public IEnumerable<InfoViewModel> Sports { get; set; }

        public int CountryId { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }
    }
}
