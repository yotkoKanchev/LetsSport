namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.UserModels;

    public class EventUser
    {
        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }


        [Required]
        public int EventId { get; set; }

        public virtual Event Event { get; set; }
    }
}
