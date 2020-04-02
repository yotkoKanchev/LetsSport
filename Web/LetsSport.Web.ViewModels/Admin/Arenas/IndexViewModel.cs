namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<InfoViewModel> Arenas { get; set; }

        public FilterBarViewModel Filter { get; set; }
    }
}
