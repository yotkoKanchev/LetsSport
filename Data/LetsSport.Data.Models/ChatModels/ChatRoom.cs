namespace LetsSport.Data.Models.ChatModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.EventModels;

    public class ChatRoom : BaseModel<string>
    {
        public ChatRoom()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
