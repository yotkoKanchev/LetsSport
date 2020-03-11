namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IArenasService
    {
        Task<int> CreateAsync(ArenaCreateInputModel inputModel);

        IEnumerable<SelectListItem> GetArenas(string city, string country);

        int GetArenaId(string name, string city, string country);

        ArenaDetailsViewModel GetDetails(int id);

        ArenaEditViewModel GetArenaForEdit(int id);

        Task UpdateArenaAsync(ArenaEditViewModel viewModel);
    }
}
