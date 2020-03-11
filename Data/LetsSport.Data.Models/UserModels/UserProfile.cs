namespace LetsSport.Data.Models.UserModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Data.Models.EventModels;

    public class UserProfile : BaseDeletableModel<string>
    {
        public UserProfile()
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

        // TODO add collection of sports
        [Required]
        public int SportId { get; set; }

        public virtual Sport Sport { get; set; }

        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public UserStatus Status { get; set; }

        // [NotMapped]
        // public int OrginizedEventsCount { get; set; }
        [MaxLength(200)]
        public string FaceBookAccount { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public string AvatarId { get; set; }

        public Image Avatar { get; set; }
    }
}
