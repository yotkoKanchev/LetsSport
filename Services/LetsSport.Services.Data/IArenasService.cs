namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IArenasService
    {
        Task<int> CreateAsync(ArenaCreateInputModel inputModel, string userId);

        IEnumerable<SelectListItem> GetArenas((string City, string Country) location);

        int GetArenaId(string name, string city, string country);

        ArenaDetailsViewModel GetDetails(int id);

        ArenaEditViewModel GetArenaForEdit(int id);

        Task UpdateArenaAsync(ArenaEditViewModel viewModel);

        Task ChangeMainImageAsync(int id, IFormFile newMainImage);

        Task DeleteMainImage(int arenaId);

        ArenaImagesEditViewModel GetArenasImagesByArenaId(int id);

        int GetArenaIdByAdminId(string arenaAdminId);
    }
}
