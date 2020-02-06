namespace LetsSport.Data.Models.ArenaModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Data.Models.EventModels;

    public class Arena : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public double PricePerHour { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string WebUrl { get; set; }

        public double Rating { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public int AddressId { get; set; }

        [Required]
        public virtual Address Address { get; set; }

        public virtual ArenaAdmin ArenaAdmin { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
