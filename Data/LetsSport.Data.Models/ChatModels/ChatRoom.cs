namespace LetsSport.Data.Models.ChatModels
{
    using System;
    using System.Collections.Generic;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;

    public class ChatRoom : BaseModel<string>
    {
        public ChatRoom()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedOn = DateTime.UtcNow;
            this.Messages = new HashSet<Message>();
            this.Users = new HashSet<UserChatRoom>();
        }

        public virtual Event Event { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<UserChatRoom> Users { get; set; }
    }
}
