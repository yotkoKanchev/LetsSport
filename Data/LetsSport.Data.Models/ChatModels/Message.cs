namespace LetsSport.Data.Models.ChatModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Message : BaseModel<string>
    {
        public Message()
        {
            this.CreatedOn = DateTime.UtcNow;
        }

        [Required]
        public string Text { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual User Sender { get; set; }

        [Required]
        public string ChatRoomId { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }
    }
}
