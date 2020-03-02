namespace LetsSport.Web.ViewModels.Arenas
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class ArenaCreateInputModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sport Type")]
        public string Sport { get; set; }

        [Display(Name = "Price per hour")]
        [Range(0, 10000000)]
        public double PricePerHour { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Web-site")]
        public string WebUrl { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string City { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public IFormFile Photo { get; set; }
    }
}
