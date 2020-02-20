namespace LetsSport.Web.ViewModels.Events
{
    public class EventDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public string Arena { get; set; }

        public string Date { get; set; }

        public string StartingHour { get; set; }

        public string Admin { get; set; }

        public string GameFormat { get; set; }

        public string Gender { get; set; }

        public double DurationInHours { get; set; }

        public double TotalPrice { get; set; }

        public int MinPlayers { get; set; }

        public int MaxPlayers { get; set; }

        public int EmptySpotsLeft { get; set; }

        public int NeededPlayersForConfirmation { get; set; }

        public string DeadLineToSendRequest { get; set; }

        public string AdditionalInfo { get; set; }

        public string Players { get; set; }

        public string Status { get; set; }

        public string RequestStatus { get; set; }
    }
}
