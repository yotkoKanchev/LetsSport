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
        private readonly string noAvatarUrl = "v1583862457/noImages/noAvatar_ppq2gm.png";


        public ImagesService(Cloudinary cloudinary, IConfiguration configuration, IDeletableEntityRepository<Image> imagesRepository)
        {
            this.cloudinary = cloudinary;
            this.configuration = configuration;
            this.imagesRepository = imagesRepository;
            this.cloudinary = cloudinary;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<string> CreateAsync(IFormFile imageSource, string noImageUrl)
        {
            var image = await this.CreateImageAsync(imageSource, noImageUrl);

            await this.imagesRepository.AddAsync(image);
            await this.imagesRepository.SaveChangesAsync();

            return image.Id;
        }

        public async Task<ICollection<Image>> CreateImagesCollectionAsync(ICollection<IFormFile> urls, string noImageUrl)
        {
            var images = new List<Image>();

            foreach (var url in urls)
            {
                var image = await this.CreateImageAsync(url, noImageUrl);
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

        public async Task ChangeImageAsync(IFormFile newImage, string id)
        {
            var image = this.imagesRepository
                .All()
                .Where(i => i.Id == id)
                .FirstOrDefault();

            var currentUrl = image.Url;

            if (newImage != null)
            {
                var url = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, newImage);
                var shortedUrl = url.Replace(this.imagePathPrefix, string.Empty);

                image.Url = shortedUrl;
                this.imagesRepository.Update(image);
                await this.imagesRepository.SaveChangesAsync();
                await ApplicationCloudinary.DeleteFile(this.cloudinary, currentUrl);
            }
        }

        public async Task DeleteImageAsync(string id, string noImageUrl)
        {
            var image = this.imagesRepository
                 .All()
                 .FirstOrDefault(i => i.Id == id);

            var currentUrl = image.Url;
            image.Url = noImageUrl;
            this.imagesRepository.Update(image);
            await this.imagesRepository.SaveChangesAsync();
            await ApplicationCloudinary.DeleteFile(this.cloudinary, currentUrl);
        }

        public async Task<string> CreateDefaultAvatarImageAsync()
        {
            var defaultAvatarImage = new Image
            {
                Url = this.noAvatarUrl,
            };

            await this.imagesRepository.AddAsync(defaultAvatarImage);
            await this.imagesRepository.SaveChangesAsync();

            return defaultAvatarImage.Id;
        }

        private async Task<Image> CreateImageAsync(IFormFile imageSource, string noImageUrl)
        {
            string url = noImageUrl;

            if (imageSource != null)
            {
                var compleateUrl = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, imageSource);
                url = compleateUrl.Replace(this.imagePathPrefix, string.Empty);
            }

            var image = new Image { Url = url };

            return image;
        }
    }
}
