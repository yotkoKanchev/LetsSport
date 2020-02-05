namespace LetsSport.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using LetsSport.Data.Common.Models;

    public class Country : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(3)]
        public string CountryCode { get; set; }

        public ICollection<City> Cities { get; set; } = new HashSet<City>();
    }
}
