namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterBarArenasPartialViewModel
    {
        public int? City { get; set; }

        public int? Sport { get; set; }

        // public string Country { get; set; }
        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
