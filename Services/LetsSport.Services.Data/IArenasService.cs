namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Admin.Arenas;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IArenasService
    {
        Task CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username);

        T GetDetails<T>(int id);

        ArenaEditViewModel GetArenaForEdit(int id);

        Task UpdateArenaAsync(ArenaEditViewModel viewModel);

        IEnumerable<SelectListItem> GetAllArenasInCitySelectList(int? cityId);

        IEnumerable<T> GetAllInCity<T>((string City, string Country) location);

        IEnumerable<SelectListItem> GetAllArenas((string City, string Country) location);

        Task<ArenaIndexListViewModel> FilterArenasAsync(int countryId, int sport, int city);

        IEnumerable<string> GetImagesUrslById(int id);

        ArenaImagesEditViewModel GetArenaImagesByArenaId(int id);

        string SetMainImage(string imageUrl);

        Task ChangeMainImageAsync(int arenaId, IFormFile newMainImage);

        Task DeleteMainImageAsync(int arenaId);

        Task AddImagesAsync(IEnumerable<IFormFile> newImages, int arenaId);

        int GetArenaIdByAdminId(string arenaAdminId);

        bool IsArenaExists(string userId);

        bool CheckUserIsArenaAdmin(string id);

        // Admin
        Task<IndexViewModel> FilterArenasByCountryIdAsync(int country);

        IEnumerable<T> GetAllInCountryAsIQueryable<T>(int countryId);

        Task<IndexViewModel> FilterArenasAsync(int countryId, int? cityId, int? sportId, int? isDeleted);

        public T GetArenaById<T>(int id);

        Task AdminUpdateArenaAsync(EditViewModel inputModel);

        Task DeleteByIdAsync(int id);
    }
}
