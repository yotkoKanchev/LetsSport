namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.SporterModels;

    public class ChatRoomSporter
    {
        [Required]
        public virtual Sporter Sporter { get; set; }

        public int SporterId { get; set; }

        [Required]
        public virtual ChatRoom ChatRoom { get; set; }

        public int ChatRoomId { get; set; }
    }
}
