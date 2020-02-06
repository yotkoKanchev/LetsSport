﻿namespace LetsSport.Data.Models.EventModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.SporterModels;

    public class Event : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public SportType SportType { get; set; }

        [Required]
        public int MinPlayers { get; set; }

        [Required]
        public int MaxPlayers { get; set; }

        [NotMapped]
        public int EmptySpotsLeft => this.MaxPlayers - this.Sporters.Count;

        [NotMapped]
        public int NeededPlayersForConfirmation => this.MinPlayers - this.Sporters.Count;

        public Gender Gender { get; set; }

        [MaxLength(100)]
        public string GameFormat { get; set; }

        [ForeignKey(nameof(Admin))] // TODO add FK with fluentAPI
        public virtual Sporter Admin { get; set; }

        public int AdminId { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }

        public int ChatRoomId { get; set; }

        public virtual Arena Arena { get; set; }

        public int ArenaId { get; set; }

        public double DurationInHours { get; set; }

        public double TotalPrice => Math.Round(this.DurationInHours * this.Arena.PricePerHour, 2);

        public DateTime DeadLineToSendRequest => this.StartingHour.AddDays(-2);

        public DateTime StartingHour { get; set; }

        public DateTime EndingHour { get; set; }

        [MaxLength(2000)]
        public string AdditionalInfo { get; set; }

        public EventStatus Status { get; set; }

        public ArenaRequestStatus RequestStatus { get; set; }

        public virtual ICollection<EventSporter> Sporters { get; set; } = new HashSet<EventSporter>();
    }
}