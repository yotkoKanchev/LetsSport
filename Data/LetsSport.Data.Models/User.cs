namespace LetsSport.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    using static LetsSport.Common.GlobalConstants;

    public class User : BaseDeletableModel<string>
    {
        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(MaximumUserNameLength)]
        public string LastName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public int? Age { get; set; }

        [MaxLength(100)]
        public string FaceBookAccount { get; set; }

        [MaxLength(200)]
        public string AvatarUrl { get; set; }

        public ICollection<SportType> Sports { get; set; } = new HashSet<SportType>();

        public UserStatus Status { get; set; }

        public int OrginizedEventsCount { get; set; }

        public ICollection<Event> Events { get; set; } = new HashSet<Event>();

        //public ICollection<ChatRoom> ChatRooms { get; set; } = new HashSet<ChatRoom>();
    }
}
