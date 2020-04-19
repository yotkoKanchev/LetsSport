namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class EditViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
