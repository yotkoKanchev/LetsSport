namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;

    public class EventUser
    {
        [Required]
        public virtual User User { get; set; }

        public int UserId { get; set; }

        [Required]
        public virtual Event Event { get; set; }

        public int EventId { get; set; }
    }
}
