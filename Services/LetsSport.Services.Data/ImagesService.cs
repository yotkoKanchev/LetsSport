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
        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_qjeerp.png";

        public ImagesService(Cloudinary cloudinary, IConfiguration configuration, IDeletableEntityRepository<Image> imagesRepository)
        {
            this.cloudinary = cloudinary;
            this.configuration = configuration;
            this.imagesRepository = imagesRepository;
            this.cloudinary = cloudinary;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<string> CreateAsync(IFormFile imageSource)
        {
            var image = await this.CreateImageAsync(imageSource);

            await this.imagesRepository.AddAsync(image);
            await this.imagesRepository.SaveChangesAsync();

            return image.Id;
        }

        public async Task<ICollection<Image>> CreateCollectionOfPicturesAsync(ICollection<IFormFile> urls)
        {
            var images = new List<Image>();

            foreach (var url in urls)
            {
                var image = await this.CreateImageAsync(url);
                images.Add(image);
            }

            return images;
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

        public async Task ChangeImageAsync(IFormFile newAvatarImage, string id)
        {
            var image = this.imagesRepository
                .All()
                .Where(i => i.Id == id)
                .FirstOrDefault();

            var currentUrl = image.Url;

            var url = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, newAvatarImage);
            var shortedUrl = url.Replace(this.imagePathPrefix, string.Empty);

            image.Url = shortedUrl;

            this.imagesRepository.Update(image);
            await this.imagesRepository.SaveChangesAsync();
            await ApplicationCloudinary.DeleteFile(this.cloudinary, currentUrl);
        }

        public async Task DeleteImageAsync(string id)
        {
            var image = this.imagesRepository
                 .All()
                 .FirstOrDefault(i => i.Id == id);

            image.Url = this.noAvatarUrl;
            this.imagesRepository.Update(image);
            await this.imagesRepository.SaveChangesAsync();
        }

        private async Task<Image> CreateImageAsync(IFormFile imageSource)
        {
            var url = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, imageSource);
            var shortedUrl = url.Replace(this.imagePathPrefix, string.Empty);

            var image = new Image { Url = shortedUrl };

            return image;
        }
    }
}
