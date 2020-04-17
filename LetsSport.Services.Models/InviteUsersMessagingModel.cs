namespace LetsSport.Services.Models
{
    using System;

    public class InviteUsersMessagingModel
    {
        public string Username { get; set; }

        public string EventName { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingTime { get; set; }

        public string ArenaName { get; set; }

        public int ArenaCityId { get; set; }

        public string Sport { get; set; }

        public int SportId { get; set; }
    }
}
