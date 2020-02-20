namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Web.ViewModels.Events;
    public interface IEventsService
    {
        Task CreateAsync(EventCreateInputModel inputModel);
    }
}
