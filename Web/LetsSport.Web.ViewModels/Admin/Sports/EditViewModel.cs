namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class EditViewModel : IMapFrom<Sport>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Url]
        [Required]
        public string Image { get; set; }
    }
}
