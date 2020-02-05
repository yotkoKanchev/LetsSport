namespace LetsSport.Data.Models.ChatModels
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.UserModels;

    public class Message : BaseDeletableModel<string>
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public virtual User User { get; set; }

        public int UserId { get; set; }
    }
}
