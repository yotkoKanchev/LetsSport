namespace LetsSport.Data.Models.Mappings
{

    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.UserModels;

    public class ChatRoomUser
    {
        [Required]
        public virtual User User { get; set; }

        public int UserId { get; set; }

        [Required]
        public virtual ChatRoom ChatRoom { get; set; }

        public int ChatRoomId { get; set; }
    }
}
