namespace LetsSport.Data.Models.AddressModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public Country()
        {
            this.CreatedOn = DateTime.UtcNow;
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();
    }
}
