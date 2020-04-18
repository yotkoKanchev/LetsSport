namespace LetsSport.Web.ViewModels.Admin
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterBarViewModel
    {
        public int CountryId { get; set; }

        public int? CityId { get; set; }

        public int? SportId { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
