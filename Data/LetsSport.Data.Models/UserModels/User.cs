namespace LetsSport.Data.Models.UserModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;

    public class User : ApplicationUser
    {
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(100)]
        public string FaceBookAccount { get; set; }

        [MaxLength(200)]
        public string AvatarUrl { get; set; }

        public UserStatus Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public virtual ICollection<Event> AdministratingEvents { get; set; } = new HashSet<Event>();

        public virtual ICollection<EventUser> Events { get; set; } = new HashSet<EventUser>();

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
