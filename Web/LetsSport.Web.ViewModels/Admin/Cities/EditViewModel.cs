namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditViewModel : IMapFrom<City>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Display(Name="Country")]
        public int CountryId { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
