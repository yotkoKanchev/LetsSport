namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    public class ImagesService : IImagesService
    {
        private readonly Cloudinary cloudinary;
        private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Image> imagesRepository;

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";

        public ImagesService(Cloudinary cloudinary, IConfiguration configuration, IDeletableEntityRepository<Image> imagesRepository)
        {
            this.cloudinary = cloudinary;
            this.configuration = configuration;
            this.imagesRepository = imagesRepository;
            this.cloudinary = cloudinary;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<Image> CreateAsync(IFormFile imageSource)
        {
            var compleateUrl = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, imageSource);
            var url = compleateUrl.Replace(this.imagePathPrefix, string.Empty);
            var image = new Image { Url = url };

            await this.imagesRepository.AddAsync(image);
            await this.imagesRepository.SaveChangesAsync();

            return image;
        }

        public string ConstructUrlPrefix(string mainImageSizing)
        {
            return this.imagePathPrefix + mainImageSizing;
        }

        public IEnumerable<string> ConstructUrls(string imageSizing, List<string> shortenedUrls)
        {
            var urls = new List<string>();

            foreach (var shortUlr in shortenedUrls)
            {
                urls.Add(this.ConstructUrlPrefix(imageSizing) + shortUlr);
            }

            return urls;
        }

        public async Task DeleteImageAsync(string id)
        {
            var image = this.imagesRepository
                 .All()
                 .FirstOrDefault(i => i.Id == id);

            if (image != null)
            {
                var avatarUrl = image.Url;
                this.imagesRepository.Delete(image);
                await this.imagesRepository.SaveChangesAsync();
                await ApplicationCloudinary.DeleteFile(this.cloudinary, avatarUrl);
            }
        }
    }
}
