namespace LetsSport.Services.Data.Cloudinary
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IApplicationCloudinary
    {
        Task<string> UploadFileAsync(IFormFile file);

        Task DeleteFile(string url);
    }
}
