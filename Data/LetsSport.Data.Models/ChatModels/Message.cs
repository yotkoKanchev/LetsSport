namespace LetsSport.Data.Models.ChatModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.SporterModels;

    public class Message : BaseDeletableModel<string>
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public virtual Sporter Sporter { get; set; }

        public int SporterId { get; set; }

        [Required]
        public virtual ChatRoom ChatRoom { get; set; }

        public int ChatRoomId { get; set; }
    }
}
