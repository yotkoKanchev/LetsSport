namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ImagesServiceTests : BaseServiceTests
    {
        private const string TestImagePath = "Test.jpg";
        private const string InvalidTestImagePath = "Test.docx";
        private const string TestImageContentType = "image/jpg";

        private IImagesService Service => this.ServiceProvider.GetRequiredService<IImagesService>();

        [Fact]
        public async Task CreateAsyncShouldAddImageToDb()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.CreateAsync(file);

            Assert.Equal(1, this.DbContext.Images.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsIfInvalidFileExtension()
        {
            using var stream = File.OpenRead(InvalidTestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await Assert.ThrowsAsync<FormatException>(() => this.Service.CreateAsync(file));
        }

        [Fact]
        public void ConstructUrlPrefixReturnsCorrectResult()
        {
            var result = this.Service.ConstructUrlPrefix("test.jpg");
            Assert.EndsWith("test.jpg", result);
            Assert.StartsWith("https://", result);
            Assert.Contains(this.Configuration["Cloudinary:ApiName"], result);
            Assert.StartsWith(string.Format(GlobalConstants.CloudinaryPrefix, this.Configuration["Cloudinary:ApiName"]), result);
        }

        [Fact]
        public void ConstructUrlsReturnsCorrectUrls()
        {
            var urls = new List<string> { "firstUrl", "secondUrl", "thirdUrl", };

            var imageSizing = "imageSizing";

            var resultUrls = this.Service.ConstructUrls(imageSizing, urls);

            foreach (var url in resultUrls)
            {
                Assert.EndsWith("Url", url);
                Assert.StartsWith("https://", url);
                Assert.Contains(this.Configuration["Cloudinary:ApiName"], url);
                Assert.StartsWith(string.Format(GlobalConstants.CloudinaryPrefix, this.Configuration["Cloudinary:ApiName"]), url);
            }
        }

        [Fact]
        public async Task DeleteByIdAsyncRemoveImageFromDb()
        {
            using var stream = File.OpenRead(TestImagePath);
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = TestImageContentType,
            };

            await this.Service.CreateAsync(file);
            Assert.Equal(1, this.DbContext.Images.Count());
            var imageId = this.DbContext.Images.Select(i => i.Id).First();
            await this.Service.DeleteByIdAsync(imageId);
            Assert.Equal(0, this.DbContext.Images.Count());
        }

        [Fact]
        public async Task DeleteByIdAsyncThrowsIfInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(()
                => this.Service.DeleteByIdAsync("id"));
        }
    }
}
