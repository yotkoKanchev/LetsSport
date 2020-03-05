namespace LetsSport.Data.Models.UserModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using LetsSport.Data.Common.Models;

    public class UserProfile : BaseDeletableModel<string>
    {
        public UserProfile()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedOn = DateTime.UtcNow;
        }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(300)]
        public string AvatarUrl { get; set; }

        public UserStatus Status { get; set; }

        [NotMapped]
        public int OrginizedEventsCount { get; set; }

        [MaxLength(200)]
        public string FaceBookAccount { get; set; }
    }
}
