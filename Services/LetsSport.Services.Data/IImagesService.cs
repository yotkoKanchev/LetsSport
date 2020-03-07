namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        public Task<string> CreateAsync(IFormFile imageSource);

        public Task<ICollection<Image>> CreateCollectionOfPicturesAsync(ICollection<IFormFile> pictures);

        public string ConstructUrlPrefix(string mainImageSizing);

        public IEnumerable<string> ConstructUrls(string imageSizing, List<string> shortenedUrls);

        public Task ChangeImageAsync(IFormFile newAvatarImage, string id);
    }
}
