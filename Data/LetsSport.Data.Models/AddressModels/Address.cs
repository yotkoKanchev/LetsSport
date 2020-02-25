namespace LetsSport.Data.Models.AddressModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;

    public class Address : BaseDeletableModel<int>
    {
        public Address()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.IsDeleted = false;
        }

        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string StreetAddress { get; set; }

        [Required]
        public int CityId { get; set; }

        public virtual City City { get; set; }

        public Arena Arena { get; set; }
    }
}
