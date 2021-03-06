﻿namespace LetsSport.Data.Models.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.Arenas;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.Users;

    public class Event : BaseModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public Gender Gender { get; set; }

        [MaxLength(100)]
        public string GameFormat { get; set; }

        public double DurationInHours { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingHour { get; set; }

        [MaxLength(2000)]
        public string AdditionalInfo { get; set; }

        public EventStatus Status { get; set; }

        public ArenaRequestStatus RequestStatus { get; set; }

        // nav props
        [Required]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        [Required]
        public int CityId { get; set; }

        public virtual City City { get; set; }

        [Required]
        public int ArenaId { get; set; }

        public virtual Arena Arena { get; set; }

        [Required]
        public int SportId { get; set; }

        public virtual Sport Sport { get; set; }

        [Required]
        public string AdminId { get; set; }

        public virtual ApplicationUser Admin { get; set; }

        public virtual ArenaRentalRequest ArenaRentalRequest { get; set; }

        public virtual ICollection<EventUser> Users { get; set; } = new HashSet<EventUser>();

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
