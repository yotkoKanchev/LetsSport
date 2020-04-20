namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenaEditViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        public string ArenaAdminId { get; set; }

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

        [Required]
        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web address")]
        public string WebUrl { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ArenaStatus Status { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        [Display(Name = "Street Address")]
        public string Address { get; set; }

        // to change arena city and country sounds strange
        // public int CityId { get; set; }
        // public int CountryId { get; set; }
        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
