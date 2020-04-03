namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<InfoViewModel> Cities { get; set; }

        public FilterBarViewModel Filter { get; set; }
    }
}
