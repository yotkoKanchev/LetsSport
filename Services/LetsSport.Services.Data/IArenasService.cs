namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IArenasService
    {
        Task<int> CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username);

        IEnumerable<T> GetAll<T>((string City, string Country) location);

        int GetArenaId(string name, string city, string country);

        ArenaDetailsViewModel GetDetails(int id);

        MyArenaDetailsViewModel GetMyArenaDetails(int id);

        IEnumerable<string> GetImageUrslById(int id);

        ArenaEditViewModel GetArenaForEdit(int id);

        Task UpdateArenaAsync(ArenaEditViewModel viewModel);

        Task ChangeMainImageAsync(int id, IFormFile newMainImage);

        Task DeleteMainImage(int arenaId);

        ArenaImagesEditViewModel GetArenasImagesByArenaId(int id);

        int GetArenaIdByAdminId(string arenaAdminId);

        Task AddImages(IEnumerable<IFormFile> newImages, int arenaId);

        bool IsArenaExists(string userId);

        IEnumerable<SelectListItem> GetAllArenas((string City, string Country) location);

        IEnumerable<ArenaIndexInfoViewModel> GetArenasByCityId(int city);
    }
}
