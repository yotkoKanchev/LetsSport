namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;

    public class EventUser
    {
        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
