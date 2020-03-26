namespace LetsSport.Web.ViewModels.Events
{
    using System.Collections.Generic;

    using LetsSport.Web.ViewModels.Messages;

    public class ChatRoomPartialViewModel
    {
        public int EventId { get; set; }

        public string Sport { get; set; }

        public string SportImage { get; set; }

        public string UserId { get; set; }

        // public string Username { get; set; }
        public string MessageContent { get; set; }

        public IEnumerable<MessageDetailsViewModel> ChatRoomMessages { get; set; }
    }
}
