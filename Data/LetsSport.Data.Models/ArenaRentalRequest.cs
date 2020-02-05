﻿namespace LetsSport.Data.Models
{
    using System;

    using LetsSport.Data.Common.Models;

    public class ArenaRentalRequest : BaseDeletableModel<string>
    {
        public virtual User User { get; set; }

        public int UserId { get; set; }

        public virtual Arena Arena { get; set; }

        public int ArenaId { get; set; }

        public DateTime SentOn { get; set; }

        public DateTime RequestResponceDate { get; set; }

        public ArenaRentingRequstStatus Status { get; set; }
    }
}
