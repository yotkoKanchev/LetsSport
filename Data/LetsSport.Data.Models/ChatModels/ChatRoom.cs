namespace LetsSport.Data.Models.ChatModels
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;

    public class ChatRoom : BaseDeletableModel<string>
    {
        public virtual Event Event { get; set; }

        public int EventId { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

        public virtual Stack<Message> Messages { get; set; } = new Stack<Message>();
    }
}
