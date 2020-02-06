namespace LetsSport.Data.Models.UserModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.EventModels;

    public class User : BaseUser
    {
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(100)]
        public string FaceBookAccount { get; set; }

        [MaxLength(200)]
        public string AvatarUrl { get; set; }

        public ICollection<SportType> Sports { get; set; } = new HashSet<SportType>();

        public UserStatus Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();

        public virtual ICollection<Event> AdministratingEvents { get; set; } = new HashSet<Event>();

        public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new HashSet<ChatRoom>();
    }
}
