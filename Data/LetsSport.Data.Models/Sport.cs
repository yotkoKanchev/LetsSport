namespace LetsSport.Data.Models
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;

    public class Sport : BaseModel<int>
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public virtual Arena Arena { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
