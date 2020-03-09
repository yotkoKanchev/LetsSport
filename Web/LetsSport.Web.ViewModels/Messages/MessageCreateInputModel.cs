namespace LetsSport.Web.ViewModels.Messages
{
    using System.ComponentModel.DataAnnotations;

    public class MessageCreateInputModel
    {
        [Required]
        [MinLength(1)]
        public string MessageContent { get; set; }
    }
}
