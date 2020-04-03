namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateInputModel
    {
        public int CountryId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
