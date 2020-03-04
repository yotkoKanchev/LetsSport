namespace LetsSport.Services.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;

    public static class ApplicationCloudinary
    {
        public static async Task<string> UploadFileAsync(Cloudinary cloudinary, IFormFile file, string fileName)
        {
            byte[] destinationFile;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                destinationFile = memoryStream.ToArray();
            }

            using (var ms = new MemoryStream(destinationFile))
            {
                // Cloudinary doesn't work with &
                var guid = Guid.NewGuid().ToString();
                fileName = fileName.Replace("&", "And") + "_" + guid;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, ms),
                    PublicId = fileName,
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUri.AbsoluteUri;
            }
        }

        public static async Task<IEnumerable<string>> UploadFilesAsync(Cloudinary cloudinary, ICollection<IFormFile> files, string fileName)
        {
            var resultList = new List<string>();
            var guid = Guid.NewGuid().ToString();
            var postfixer = 1;

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
                    // Cloudinary doesn't work with &
                    fileName = fileName.Replace("&", "And") + "_" + guid + postfixer;
                    postfixer++;

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(fileName, ms),
                        PublicId = fileName,
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
