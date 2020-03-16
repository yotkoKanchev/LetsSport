namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        Task<string> CreateAsync(IFormFile imageSource, string noImageUrl);

        Task<ICollection<Image>> CreateImagesCollectionAsync(ICollection<IFormFile> pictures, string noImageUrl);

        string ConstructUrlPrefix(string mainImageSizing);

        IEnumerable<string> ConstructUrls(string imageSizing, List<string> shortenedUrls);

        Task ChangeImageAsync(IFormFile newAvatarImage, string id);

        Task DeleteImageAsync(string id, string noImageUrl);

        Task<string> CreateDefaultAvatarImageAsync();
    }
}
