namespace LetsSport.Data.Models.ArenaModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;

    public class ArenaRentalRequest : BaseDeletableModel<string>
    {
        public DateTime SentOnDate { get; set; }

        public DateTime RequestResponceDate { get; set; }

        public ArenaRentalRequestStatus Status { get; set; }

        [Required]
        public int ArenaId { get; set; }

        public virtual Arena Arena { get; set; }

        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
