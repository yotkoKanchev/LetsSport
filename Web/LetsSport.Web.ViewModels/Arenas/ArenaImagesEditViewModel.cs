namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;

    using LetsSport.Common;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Images;
    using LetsSport.Web.ViewModels.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class ArenaImagesEditViewModel : IMapFrom<Arena>
    {
        public IEnumerable<ImageInfoViewModel> Images { get; set; }

        [AllowedExtensions]
        [MaxFileSize(GlobalConstants.ImageMaxSizeMB * 1024 * 1024)]
        public IEnumerable<IFormFile> NewImages { get; set; }
    }
}
