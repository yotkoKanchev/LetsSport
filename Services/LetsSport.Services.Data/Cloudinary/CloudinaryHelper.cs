namespace LetsSport.Services.Data.Cloudinary
{
    using LetsSport.Common;
    using Microsoft.Extensions.Configuration;

    public class CloudinaryHelper : ICloudinaryHelper
    {
        private readonly IConfiguration configuration;

        public CloudinaryHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetPrefix()
        {
            var apiName = this.configuration["Cloudinary:ApiName"];
            return string.Format(GlobalConstants.CloudinaryPrefix, apiName);
        }
    }
}
