namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;

    public interface IArenasService
    {
        Task<int> CreateAsync(ArenaCreateInputModel inputModel);

        IEnumerable<string> GetArenas();

        int GetArenaId(string name);

        ArenaDetailsViewModel GetDetails(int id);

        ArenaEditViewModel GetArenaForEdit(int id);

        Task UpdateArenaAsync(ArenaEditViewModel viewModel);
    }
}
