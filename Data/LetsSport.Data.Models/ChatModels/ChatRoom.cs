namespace LetsSport.Data.Models.ChatModels
{
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;

    public class ChatRoom : BaseDeletableModel<string>
    {
        public virtual Event Event { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
