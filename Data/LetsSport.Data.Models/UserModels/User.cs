namespace LetsSport.Data.Models.UserModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.UserModels;

    public class User : BaseDeletableModel<string>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
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
