namespace LetsSport.Data.Models
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;

    public class ChatRoom : BaseDeletableModel<string>
    {
        public virtual Event Event { get; set; }

        public int EventId { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();

        public virtual Stack<Message> Messages { get; set; } = new Stack<Message>();
    }
}
