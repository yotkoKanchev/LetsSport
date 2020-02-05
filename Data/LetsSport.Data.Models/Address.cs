namespace LetsSport.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Address : BaseDeletableModel<int>
    {
        [Required]
        public Country Country { get; set; }

        public int CountryId { get; set; }

        [Required]
        public City City { get; set; }

        public int CityId { get; set; }

        public Neighborhood Neighborhood { get; set; }

        public int NeighborhoodId { get; set; }

        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string StreetAddress { get; set; }
    }
}
