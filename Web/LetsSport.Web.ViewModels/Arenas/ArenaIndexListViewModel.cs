namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Shared;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenaIndexListViewModel
    {
        public IEnumerable<_ArenaCardPartialViewModel> Arenas { get; set; }

        public int City { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
    }
}
