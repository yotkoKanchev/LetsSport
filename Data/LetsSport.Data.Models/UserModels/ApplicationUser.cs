namespace LetsSport.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        // Additional info
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        public UserStatus Status { get; set; }

        [MaxLength(200)]
        public string FaceBookAccount { get; set; }

        [MaxLength(100)]
        public string Occupation { get; set; }

        public bool IsUserProfileUpdated { get; set; }

        // nav props
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public int? CityId { get; set; }

        public virtual City City { get; set; }

        public int? SportId { get; set; }

        public virtual Sport Sport { get; set; }

        public string AvatarId { get; set; }

        public Image Avatar { get; set; }

        public virtual Arena AdministratingArena { get; set; }

        public virtual ICollection<Event> AdministratingEvents { get; set; } = new HashSet<Event>();

        public virtual ICollection<EventUser> Events { get; set; } = new HashSet<EventUser>();

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
