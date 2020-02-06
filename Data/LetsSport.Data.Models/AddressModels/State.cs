namespace LetsSport.Data.Models.AddressModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class State : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public virtual Country Country { get; set; }

        public int CountryId { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();
    }
}
