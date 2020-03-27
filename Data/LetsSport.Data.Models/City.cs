namespace LetsSport.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;

    public class City : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        // TODO add State nav prop
        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>(); //

        public virtual ICollection<Arena> Arenas { get; set; } = new HashSet<Arena>(); //

        public virtual ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>(); //
    }
}
