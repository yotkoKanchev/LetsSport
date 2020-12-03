namespace LetsSport.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.Events;
    using LetsSport.Data.Models.Users;

    public class Message : BaseModel<string>
    {
        public Message()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Content { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
