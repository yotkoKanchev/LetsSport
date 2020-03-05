namespace LetsSport.Data.Models.AddressModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.UserModels;

    public class City : BaseModel<int>
    {
        public City()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Addresses = new HashSet<Address>();
            this.Users = new HashSet<UserProfile>();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public virtual ICollection<UserProfile> Users { get; set; }
    }
}
