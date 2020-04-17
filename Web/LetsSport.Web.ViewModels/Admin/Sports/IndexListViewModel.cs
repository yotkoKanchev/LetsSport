namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using System.Collections.Generic;

    public class IndexListViewModel
    {
        public IEnumerable<SportInfoViewModel> Sports { get; set; }

        public int CountryId { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }
    }
}
