namespace LetsSport.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Arena : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public Address Address { get; set; }

        public int AddressId { get; set; }

        public string PricePerHour { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string WebUrl { get; set; }

        public double Rating { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
    }
}
