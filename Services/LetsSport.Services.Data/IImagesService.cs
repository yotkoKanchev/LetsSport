namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        Task<Image> CreateAsync(IFormFile imageSource);

        string ConstructUrlPrefix(string mainImageSizing);

        IEnumerable<string> ConstructUrls(string imageSizing, List<string> shortenedUrls);

        Task<string> ChangeImageAsync(IFormFile newAvatarImage, string id);

        Task DeleteImageAsync(string id);

        string GetArenaAdminIdByImageId(string id);
    }
}
