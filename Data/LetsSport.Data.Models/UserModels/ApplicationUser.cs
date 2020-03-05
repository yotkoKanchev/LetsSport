// ReSharper disable VirtualMemberCallInConstructor
namespace LetsSport.Data.Models
{
    using System;
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.IsEventAdmin = false;
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.AdministratingEvents = new HashSet<Event>();
            this.Events = new HashSet<EventUser>();
            this.Messages = new HashSet<Message>();
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
        public bool IsEventAdmin { get; set; }

        public virtual ICollection<Event> AdministratingEvents { get; set; }

        public virtual ICollection<EventUser> Events { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public int? AdministratingArenaId { get; set; }

        public virtual Arena AdministratingArena { get; set; }

        public virtual UserProfile User { get; set; }

        public virtual ArenaAdmin ArenaAdmin { get; set; }
    }
}
