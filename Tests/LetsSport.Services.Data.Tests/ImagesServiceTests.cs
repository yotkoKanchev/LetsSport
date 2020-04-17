namespace LetsSport.Services.Data.Tests
{
    using CloudinaryDotNet;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;

    public class ImagesServiceTests : BaseServiceTests
    {
        public ImagesServiceTests()
        {
            var cloudinaryMock = new Mock<Cloudinary>();
            var imagePlaceholderUrl = "https://steamcdn-a.akamaihd.net/steam/apps/440/header.jpg";

            //cloudinaryMock
            //    .Setup(x => x.UploadImage(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<Transformation>()))
            //    .ReturnsAsync(imagePlaceholderUrl);
        }
        private IImagesService Service => this.ServiceProvider.GetRequiredService<IImagesService>();
    }
}
