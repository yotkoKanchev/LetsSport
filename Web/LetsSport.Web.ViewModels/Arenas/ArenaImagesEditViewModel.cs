namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Images;
    using Microsoft.AspNetCore.Http;

    public class ArenaImagesEditViewModel : IMapFrom<Arena>
    {
        public IEnumerable<ImageInfoViewModel> Images { get; set; }

        public IEnumerable<IFormFile> NewImages { get; set; }
    }
}
