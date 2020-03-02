namespace LetsSport.Web.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Web.ViewModels.Messages;

    public class ChatRoomViewModel
    {
        public int EventId { get; set; }

        public string Id { get; set; }

        public string EventName { get; set; }

        [Required]
        [MinLength(1)]
        public string Text { get; set; }

        public IEnumerable<MessageDetailsViewModel> Messages { get; set; }
    }
}
