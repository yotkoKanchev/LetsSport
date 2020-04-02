namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using System.ComponentModel.DataAnnotations;

    public class CreateInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Url]
        [Required]
        public string Image { get; set; }
    }
}
