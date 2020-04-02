namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class EditViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
