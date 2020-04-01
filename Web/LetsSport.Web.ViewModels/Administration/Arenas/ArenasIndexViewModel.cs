namespace LetsSport.Web.ViewModels.Administration.Arenas
{
    using System.Collections.Generic;

    public class ArenasIndexViewModel
    {
        public IEnumerable<ArenaInfoViewModel> Arenas { get; set; }

        public ArenasFilterBarViewModel Filter { get; set; }
    }
}
