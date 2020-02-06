namespace LetsSport.Data.Models.ArenaModels
{
    using System;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.SporterModels;

    public class ArenaRentalRequest : BaseDeletableModel<string>
    {
        public virtual Sporter Sporter { get; set; }

        public string SporterId { get; set; }

        public virtual Event Event { get; set; }

        public string EventId { get; set; }

        public virtual Arena Arena { get; set; }

        public int ArenaId { get; set; }

        public DateTime SentOnDate { get; set; }

        public DateTime RequestResponceDate { get; set; }

        public ArenaRentalRequestStatus Status { get; set; }
    }
}
