namespace LetsSport.Data.Models.Mappings
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.SporterModels;

    public class EventSporter
    {
        public string SporterId { get; set; }

        [Required]
        public virtual Sporter Sporter { get; set; }

        public int EventId { get; set; }

        [Required]
        public virtual Event Event { get; set; }
    }
}
