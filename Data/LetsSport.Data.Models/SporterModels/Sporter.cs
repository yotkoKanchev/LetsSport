namespace LetsSport.Data.Models.SporterModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;

    public class Sporter : BaseUser
    {
        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(100)]
        public string FaceBookAccount { get; set; }

        [MaxLength(200)]
        public string AvatarUrl { get; set; }

        //public ICollection<SportType> Sports { get; set; } = new HashSet<SportType>();

        public SporterStatus Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public virtual ICollection<Event> AdministratingEvents { get; set; } = new HashSet<Event>();

        public virtual ICollection<EventSporter> Events { get; set; } = new HashSet<EventSporter>();

        public virtual ICollection<ChatRoomSporter> ChatRooms { get; set; } = new HashSet<ChatRoomSporter>();
    }
}
