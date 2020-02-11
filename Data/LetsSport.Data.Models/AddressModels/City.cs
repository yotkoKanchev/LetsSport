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
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
