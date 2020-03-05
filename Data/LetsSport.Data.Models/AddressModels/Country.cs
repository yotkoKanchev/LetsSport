namespace LetsSport.Data.Models.AddressModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.UserModels;

    public class Country : BaseModel<int>
    {
        public Country()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Users = new HashSet<UserProfile>();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();

        public virtual ICollection<UserProfile> Users { get; set; }
    }
}
