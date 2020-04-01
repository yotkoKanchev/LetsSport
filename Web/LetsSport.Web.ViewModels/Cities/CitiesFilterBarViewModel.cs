namespace LetsSport.Web.ViewModels.Cities
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CitiesFilterBarViewModel
    {
        public int? Country { get; set; }

        public int IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
