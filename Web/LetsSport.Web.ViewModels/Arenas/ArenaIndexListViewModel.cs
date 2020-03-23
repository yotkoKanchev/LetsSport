namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenaIndexListViewModel
    {
        public IEnumerable<ArenaIndexInfoViewModel> Arenas { get; set; }

        public int City { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
    }
}
