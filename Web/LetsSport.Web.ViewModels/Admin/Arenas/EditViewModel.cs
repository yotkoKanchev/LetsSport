namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.Arenas;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sport Type")]
        public int SportId { get; set; }

        [Display(Name = "Price per hour")]
        [Range(0, 10000000)]
        public double PricePerHour { get; set; }

        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Web-site")]
        public string WebUrl { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ArenaStatus Status { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        [Display(Name = "Street Address")]
        public string Address { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        public int CountryId { get; set; }
    }
}
