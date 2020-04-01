namespace LetsSport.Web.ViewModels.Administration.Arenas
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenasFilterBarViewModel
    {
        public int Country { get; set; }

        public int? City { get; set; }

        public int? Sport { get; set; }

        public int? IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
