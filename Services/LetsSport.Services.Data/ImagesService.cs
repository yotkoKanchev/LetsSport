namespace LetsSport.Services.Data
{
    using System;
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
        private const string InvalidImageIdErrorMessage = "Image with ID: {0} does not exist.";

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

        public async Task<string> ChangeImageAsync(IFormFile newImage, string userId)
        {
            var image = this.imagesRepository
                .All()
                .Where(i => i.User.Id == userId)
                .FirstOrDefault();

            if (image == null)
            {
                var avatar = await this.CreateAsync(newImage);
                return avatar.Id;
            }

            var currentUrl = image.Url;
            var newAvatar = await this.CreateAsync(newImage);
            this.imagesRepository.Delete(image);
            await this.imagesRepository.SaveChangesAsync();
            await ApplicationCloudinary.DeleteFile(this.cloudinary, currentUrl);

            return newAvatar.Id;
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

        public string GetArenaAdminIdByImageId(string id)
        {
            var arenaAdminId = this.imagesRepository
                .All()
                .Where(i => i.Id == id)
                //.Select(i => i.Arena.Id)
                .FirstOrDefault();

            return arenaAdminId.ToString();
        }
    }
}
