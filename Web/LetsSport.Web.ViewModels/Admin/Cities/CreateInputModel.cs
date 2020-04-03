namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.ComponentModel.DataAnnotations;

    public class CreateInputModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
