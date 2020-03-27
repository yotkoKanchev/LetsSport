namespace LetsSport.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;

    public class Country : BaseModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>(); //

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>(); //

        public virtual ICollection<Arena> Arenas { get; set; } = new HashSet<Arena>(); //

        public virtual ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>(); //

        // TODO add collection of State
    }
}
