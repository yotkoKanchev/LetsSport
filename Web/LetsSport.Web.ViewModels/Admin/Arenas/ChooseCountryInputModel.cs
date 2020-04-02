namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ChooseCountryInputModel
    {
        public int CountryId { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
