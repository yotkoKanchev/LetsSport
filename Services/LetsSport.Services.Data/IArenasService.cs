namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;

    public interface IArenasService
    {
        Task Create(ArenaCreateInputModel inputModel);

        IEnumerable<string> GetArenas();
    }
}
