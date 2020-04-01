﻿namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.Administration.Arenas;
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

        ArenaIndexListViewModel FilterArenas(string country, int sport, int city);

        IEnumerable<string> GetImagesUrslById(int id);

        ArenaImagesEditViewModel GetArenaImagesByArenaId(int id);

        string SetMainImage(string imageUrl);

        Task ChangeMainImageAsync(int arenaId, IFormFile newMainImage);

        Task DeleteMainImage(int arenaId);

        Task AddImages(IEnumerable<IFormFile> newImages, int arenaId);

        int GetArenaIdByAdminId(string arenaAdminId);

        bool IsArenaExists(string userId);

        bool CheckUserIsArenaAdmin(string id);

        // Administration
        ArenasIndexViewModel FilterArenasByCountryId(int country);

        IQueryable<Arena> GetAll();

        ArenasIndexViewModel FilterArenas(int country, int? city, int? sport, int? isDeleted);
    }
}
