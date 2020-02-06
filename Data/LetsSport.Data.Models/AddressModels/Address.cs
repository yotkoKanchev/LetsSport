namespace LetsSport.Data.Models.AddressModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Address : BaseDeletableModel<int>
    {
        public int CityId { get; set; }

        [Required]
        public virtual City City { get; set; }

        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string StreetAddress { get; set; }
    }
}
