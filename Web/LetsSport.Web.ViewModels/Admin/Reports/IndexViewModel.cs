namespace LetsSport.Web.ViewModels.Admin.Reports
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<InfoViewModel> Reports { get; set; }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }
    }
}
