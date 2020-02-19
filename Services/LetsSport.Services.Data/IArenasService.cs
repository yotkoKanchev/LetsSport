namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Arenas;

    public interface IArenasService
    {
        Task Create(ArenaCreateInputModel inputModel);
    }
}
