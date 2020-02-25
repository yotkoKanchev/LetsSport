namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;

    public class UserChatRoom
    {
        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string ChatRoomId { get; set; }

        public ChatRoom ChatRoom { get; set; }
    }
}
