namespace LetsSport.Services.Data.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;

    public static class ApplicationCloudinary
    {
        public static async Task<string> UploadFileAsync(Cloudinary cloudinary, IFormFile file)
        {
            byte[] destinationFile;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                destinationFile = memoryStream.ToArray();
            }

            ImageUploadResult uploadResult;
            using (var ms = new MemoryStream(destinationFile))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, ms),
                };

                uploadResult = await cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult.SecureUri.AbsoluteUri;
        }

        public static async Task<IEnumerable<string>> UploadFilesAsync(Cloudinary cloudinary, ICollection<IFormFile> files)
        {
            var resultList = new List<string>();

            foreach (var file in files)
            {
                byte[] destinationFile;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    destinationFile = memoryStream.ToArray();
                }

                using (var ms = new MemoryStream(destinationFile))
                {

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, ms),
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    resultList.Add(uploadResult.SecureUri.AbsoluteUri);
                }
            }

            return resultList;
        }

        public static async Task DeleteFile(Cloudinary cloudinary, string fileName)
        {
            var delParams = new DelResParams()
            {
                PublicIds = new List<string>() { fileName },
                Invalidate = true,
            };

            await cloudinary.DeleteResourcesAsync(delParams);
        }
    }
}
