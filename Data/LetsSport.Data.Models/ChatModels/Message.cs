namespace LetsSport.Data.Models.ChatModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.UserModels;

    public class Message : BaseDeletableModel<string>
    {
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
