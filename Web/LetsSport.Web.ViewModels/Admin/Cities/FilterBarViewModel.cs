namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterBarViewModel
    {
        public int? Country { get; set; }

        public int IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
