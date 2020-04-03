namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<InfoViewModel> Events { get; set; }

        public FilterBarViewModel Filter { get; set; }
    }
}
