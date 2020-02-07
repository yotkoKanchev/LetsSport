namespace LetsSport.Data.Models.AddressModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;

    public class Address : BaseDeletableModel<int>
    {
        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string StreetAddress { get; set; }

        public int CityId { get; set; }

        [Required]
        public virtual City City { get; set; }

        public Arena Arena { get; set; }
    }
}
