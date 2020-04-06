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

        Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId);

        public T GetById<T>(int id);

        int GetIdByAdminId(string arenaAdminId);

        Task CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username);

        T GetDetails<T>(int id);

        ArenaEditViewModel GetDetailsForEdit(int id);

        Task UpdateAsync(ArenaEditViewModel viewModel);

        Task<ArenaIndexListViewModel> FilterAsync(int countryId, int sport, int city);

        // imgs TODO refactor all imgs methods
        IEnumerable<string> GetImagesUrslById(int id);

        ArenaImagesEditViewModel GetImagesById(int id);

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

        int GetCountInCountry(int countryId);
    }
}
