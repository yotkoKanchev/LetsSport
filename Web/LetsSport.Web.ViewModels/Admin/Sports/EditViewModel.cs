namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class EditViewModel : IMapFrom<Sport>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
