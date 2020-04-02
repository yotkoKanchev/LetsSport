namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using System.ComponentModel.DataAnnotations;

    public class CreateInputModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
