namespace LetsSport.Web.ViewModels.Messages
{
    using System.ComponentModel.DataAnnotations;

    using static LetsSport.Common.GlobalConstants;

    public class MessageCreateInputModel
    {
        [Required]
        [MinLength(MessageContentMinLength)]
        [MaxLength(MessageContentMaxLength)]
        public string MessageContent { get; set; }
    }
}
