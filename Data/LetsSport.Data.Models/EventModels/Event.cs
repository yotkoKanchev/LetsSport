namespace LetsSport.Data.Models.EventModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;

    public class Event : BaseModel<int>
    {
        public Event()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Users = new HashSet<EventUser>();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public SportType SportType { get; set; }

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        [NotMapped]
        public int EmptySpotsLeft => this.MaxPlayers - this.Users.Count;

        [NotMapped]
        public int NeededPlayersForConfirmation => this.MinPlayers - this.Users.Count;

        [NotMapped]
        public double TotalPrice => Math.Round(this.DurationInHours * this.Arena.PricePerHour, 2);

        public Gender Gender { get; set; }

        [MaxLength(100)]
        public string GameFormat { get; set; }

        public double DurationInHours { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingHour { get; set; }

        [NotMapped]
        public DateTime EndingHour => this.StartingHour.AddHours(this.DurationInHours);

        [NotMapped]
        public DateTime DeadLineToSendRequest => this.StartingHour.AddDays(-2);

        [MaxLength(2000)]
        public string AdditionalInfo { get; set; }

        public EventStatus Status { get; set; }

        public ArenaRequestStatus RequestStatus { get; set; }

        [Required]
        public int ArenaId { get; set; }

        public virtual Arena Arena { get; set; }

        [Required]
        public string AdminId { get; set; }

        public virtual ApplicationUser Admin { get; set; }

        [Required]
        public string ChatRoomId { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }

        public virtual ArenaRentalRequest ArenaRentalRequest { get; set; }

        public virtual ICollection<EventUser> Users { get; set; }
    }
}
