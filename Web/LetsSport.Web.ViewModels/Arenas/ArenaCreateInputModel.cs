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
        public string Sport { get; set; }

        public double PricePerHour { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string WebUrl { get; set; }

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
