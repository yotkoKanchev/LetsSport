namespace LetsSport.Web.ViewModels.Admin
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterBarViewModel
    {
        public int CountryId { get; set; }

        public int? City { get; set; }

        public int? Sport { get; set; }

        public int? IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
