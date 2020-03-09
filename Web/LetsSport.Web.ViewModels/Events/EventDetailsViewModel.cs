namespace LetsSport.Web.ViewModels.Events
{
    using LetsSport.Web.ViewModels.Messages;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class EventDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Sport Type")]
        public string SportType { get; set; }

        public string Arena { get; set; }

        public string Date { get; set; }

        [DisplayName("Start time")]
        public string StartingHour { get; set; }

        public string Admin { get; set; }

        [DisplayName("Game format")]
        public string GameFormat { get; set; }

        public string Gender { get; set; }

        [DisplayName("Duration in hours")]
        public double DurationInHours { get; set; }

        [DisplayName("Total price")]
        public double TotalPrice { get; set; }

        [DisplayName("Minimum players")]
        public int MinPlayers { get; set; }

        [DisplayName("Maximum players")]
        public int MaxPlayers { get; set; }

        [DisplayName("Empty spots")]
        public int EmptySpotsLeft { get; set; }

        [DisplayName("Needed Players")]
        public int NeededPlayersForConfirmation { get; set; }

        [DisplayName("Request Deadline")]
        public string DeadLineToSendRequest { get; set; }

        [DisplayName("Additional info")]
        public string AdditionalInfo { get; set; }

        public string Players { get; set; }

        public string Status { get; set; }

        public string MessageContent { get; set; }

        [DisplayName("Request Status")]
        public string RequestStatus { get; set; }

        public IEnumerable<MessageDetailsViewModel> Messages { get; set; }
    }
}
