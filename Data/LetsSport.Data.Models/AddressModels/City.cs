namespace LetsSport.Data.Models.AddressModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class City : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public Country Country { get; set; }

        public int CountryId { get; set; }

        public ICollection<Neighborhood> Neighborhoods { get; set; } = new HashSet<Neighborhood>();

        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
