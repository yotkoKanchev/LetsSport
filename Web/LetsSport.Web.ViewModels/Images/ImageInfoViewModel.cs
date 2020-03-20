namespace LetsSport.Web.ViewModels.Images
{
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class ImageInfoViewModel : IMapFrom<Image>
    {
        public string Id { get; set; }

        public string Url { get; set; }

        // public bool Selected { get; set; }
    }
}
