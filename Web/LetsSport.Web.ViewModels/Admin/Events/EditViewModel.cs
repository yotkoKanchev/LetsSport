namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditViewModel : IMapFrom<Event>
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

        public EventStatus Status { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
