namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Images;

    public class ArenaImagesEditViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        public IEnumerable<ImageInfoViewModel> Images { get; set; }
    }
}
