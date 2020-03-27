namespace LetsSport.Data.Models.ArenaModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;

    public class Arena : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // TODO add collection of sports
        public double PricePerHour { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(260)]
        public string WebUrl { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public ArenaStatus Status { get; set; }

        // nav props
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public int SportId { get; set; }

        public virtual Sport Sport { get; set; }

        public string MainImageId { get; set; }

        public virtual Image MainImage { get; set; }

        public string ArenaAdminId { get; set; }

        public virtual ApplicationUser ArenaAdmin { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();

        public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
    }
}
