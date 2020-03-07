namespace LetsSport.Data.Models.ChatModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Message : BaseModel<string>
    {
        public Message()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Content { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public string ChatRoomId { get; set; }

        public virtual ChatRoom ChatRoom { get; set; }
    }
}
