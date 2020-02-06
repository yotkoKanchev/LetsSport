﻿namespace LetsSport.Data.Models.ArenaModels
{
    using System;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;

    public class ArenaRentalRequest : BaseDeletableModel<string>
    {
        public virtual User User { get; set; }

        public int UserId { get; set; }

        public virtual Event Event { get; set; }

        public int EventId { get; set; }

        public virtual Arena Arena { get; set; }

        public int ArenaId { get; set; }

        public DateTime SentOnDate { get; set; }

        public DateTime RequestResponceDate { get; set; }

        public ArenaRentalRequestStatus Status { get; set; }
    }
}
