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

        public int ArenaId { get; set; }

        [Required]
        public virtual Arena Arena { get; set; }

        public int EventId { get; set; }

        [Required]
        public virtual Event Event { get; set; }
    }
}
