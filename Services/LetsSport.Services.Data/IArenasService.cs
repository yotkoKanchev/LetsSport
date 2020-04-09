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
        Task<IEnumerable<SelectListItem>> GetAllActiveInCitySelectListAsync(int cityId);

        Task<int> GetCountInCityAsync(int cityId);

        Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId, int? take = null, int skip = 0);

        Task<T> GetByIdAsync<T>(int id);

        Task<int> GetIdByAdminIdAsync(string arenaAdminId);

        Task CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username);

        Task<T> GetDetailsAsync<T>(int id);

        Task<ArenaEditViewModel> GetDetailsForEditAsyc(int id);

        Task UpdateAsync(ArenaEditViewModel viewModel);

        Task<ArenaIndexListViewModel> FilterAsync(
            int countryId, int? sport, int? city, int? take = null, int skip = 0);

        // imgs TODO refactor all imgs methods
        Task<IEnumerable<string>> GetImageUrslByIdAsync(int id);

        Task<ArenaImagesEditViewModel> GetImagesByIdAsync(int id);

        string SetMainImage(string imageUrl);

        Task ChangeMainImageAsync(int arenaId, IFormFile newMainImage);

        Task DeleteMainImageAsync(int arenaId);

        Task AddImagesAsync(IEnumerable<IFormFile> newImages, int arenaId);

        // Admin
        Task<IEnumerable<T>> GetAllInCountryAsync<T>(int cityId, int? take = null, int skip = 0);

        Task<IEnumerable<SelectListItem>> GetAllInCitySelectListAsync(int? cityId);

        Task<IndexViewModel> AdminFilterAsync(int countryId, int? cityId, int? sportId, int? isDeleted, int? take = null, int skip = 0);

        Task AdminUpdateAsync(EditViewModel inputModel);

        Task DeleteByIdAsync(int id);

        Task<int> GetCountInCountryAsync(int countryId);
    }
}
