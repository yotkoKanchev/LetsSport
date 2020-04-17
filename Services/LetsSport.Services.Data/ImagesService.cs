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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ImagesService : IImagesService
    {
        private const string FileFormatErrorMessage = "File format not supported!";
        private const string InvalidImageIdErrorMessage = "Image with ID: {0} does not exists.";
        private readonly string[] validImageExtensions = { ".ai", ".gif", ".webp", ".bmp", ".djvu", ".ps", ".ept", ".eps", ".eps3", ".fbx", ".flif", ".gif", ".gltf", ".heif", ".heic", ".ico", ".indd", ".jpg", ".jpe", ".jpeg", ".jp24", ".wdp", ".jxr", ".hdp", ".pdf", ".png", ".psd", ".arw", ".cr2", ".svg", ".tga", ".tif", ".tiff", ".webp", };
        private readonly Cloudinary cloudinary;
        //private readonly IConfiguration configuration;
        private readonly IDeletableEntityRepository<Image> imagesRepository;
        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";

        public ImagesService(
            Cloudinary cloudinary,
            //IConfiguration configuration,
            IDeletableEntityRepository<Image> imagesRepository)
        {
            this.cloudinary = cloudinary;
            //this.configuration = configuration;
            this.imagesRepository = imagesRepository;
            this.cloudinary = cloudinary;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, CloudinaryConfig.ApiName /*this.configuration["Cloudinary:ApiName"]*/);
        }

        public async Task<Image> CreateAsync(IFormFile imageSource)
        {
            if (this.validImageExtensions.Any(e => imageSource.FileName.EndsWith(e)))
            {
                var compleateUrl = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, imageSource);
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
            await ApplicationCloudinary.DeleteFile(this.cloudinary, imageUrl);
        }
    }
}
