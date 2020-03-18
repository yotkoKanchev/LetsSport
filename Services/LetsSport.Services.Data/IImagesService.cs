namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        Task<string> CreateAsync(IFormFile imageSource);

        string ConstructUrlPrefix(string mainImageSizing);

        IEnumerable<string> ConstructUrls(string imageSizing, List<string> shortenedUrls);

        Task<string> ChangeImageAsync(IFormFile newAvatarImage, string id);

        Task DeleteImageAsync(string id);

        //Task<string> CreateDefaultAvatarImageAsync();
    }
}
