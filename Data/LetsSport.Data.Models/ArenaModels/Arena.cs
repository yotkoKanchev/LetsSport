namespace LetsSport.Data.Models.ArenaModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Data.Models.EventModels;

    public class Arena : BaseDeletableModel<int>
    {
        public Arena()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.IsDeleted = false;
            this.Events = new HashSet<Event>();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public SportType Sport { get; set; }

        public double PricePerHour { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(260)]
        public string WebUrl { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public string MainImage { get; set; }

        public string Pictures { get; set; }

        [NotMapped]
        public double Rating { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public int AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual ApplicationUser ArenaAdmin { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
