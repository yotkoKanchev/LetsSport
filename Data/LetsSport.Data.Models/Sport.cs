namespace LetsSport.Data.Models
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;

    public class Sport : BaseModel<int>
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();

        public virtual ICollection<Arena> Arenas { get; set; } = new HashSet<Arena>();

        public virtual ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
    }
}
