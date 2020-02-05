namespace LetsSport.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Message : BaseDeletableModel<string>
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int UserId { get; set; }
    }
}
