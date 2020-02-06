namespace LetsSport.Data.Models.ChatModels
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;

    public class ChatRoom : BaseDeletableModel<string>
    {
        public virtual Event Event { get; set; }

        public int EventId { get; set; }

        public virtual ICollection<ChatRoomSporter> Sporters { get; set; } = new HashSet<ChatRoomSporter>();

        public virtual Stack<Message> Messages { get; set; } = new Stack<Message>();
    }
}
