﻿namespace LetsSport.Web.ViewModels.ArenaRequests
{
    using System;

    using LetsSport.Data.Models.Arenas;
    using LetsSport.Data.Models.Events;
    using LetsSport.Services.Mapping;

    public class EventInfoViewModel : IMapFrom<Event>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingHour { get; set; }

        public double Duration { get; set; }

        public string AdminUserName { get; set; }

        public string AdminId { get; set; }

        public string SportName { get; set; }

        public string GameFormat { get; set; }

        public int UsersCount { get; set; }

        public ArenaRentalRequestStatus ArenaRentalRequestStatus { get; set; }
    }
}
