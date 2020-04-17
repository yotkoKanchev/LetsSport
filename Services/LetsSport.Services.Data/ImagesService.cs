namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Cloudinary;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;
    using static LetsSport.Common.GlobalConstants;

    public class ImagesService : IImagesService
    {
        private readonly string[] validImageExtensions = { ".ai", ".gif", ".webp", ".bmp", ".djvu", ".ps", ".ept", ".eps", ".eps3", ".fbx", ".flif", ".gif", ".gltf", ".heif", ".heic", ".ico", ".indd", ".jpg", ".jpe", ".jpeg", ".jp24", ".wdp", ".jxr", ".hdp", ".pdf", ".png", ".psd", ".arw", ".cr2", ".svg", ".tga", ".tif", ".tiff", ".webp", };
        private readonly IApplicationCloudinary cloudinary;
        private readonly IDeletableEntityRepository<Image> imagesRepository;
        private readonly string imagePathPrefix;

        public ImagesService(
            IApplicationCloudinary cloudinary,
            IDeletableEntityRepository<Image> imagesRepository)
        {
            this.cloudinary = cloudinary;
            this.imagesRepository = imagesRepository;
            this.cloudinary = cloudinary;
            this.imagePathPrefix = string.Format(CloudinaryPrefix, CloudinaryConfig.ApiName);
        }

        public async Task<Image> CreateAsync(IFormFile imageSource)
        {
            if (this.validImageExtensions.Any(e => imageSource.FileName.EndsWith(e)))
            {
                var compleateUrl = await this.cloudinary.UploadFileAsync(imageSource);
                var url = compleateUrl.Replace(this.imagePathPrefix, string.Empty);
                var image = new Image { Url = url };

                await this.imagesRepository.AddAsync(image);
                await this.imagesRepository.SaveChangesAsync();

                return image;
            }

            throw new FormatException(FileFormatErrorMessage);
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

        public async Task DeleteAsync(string id)
        {
            var image = await this.imagesRepository
                 .All()
                 .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                throw new ArgumentException(string.Format(InvalidImageIdErrorMessage, id));
            }

            var imageUrl = image.Url;
            this.imagesRepository.Delete(image);
            await this.imagesRepository.SaveChangesAsync();
            await this.cloudinary.DeleteFile(imageUrl);
        }
    }
}
