namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.ArenaRequests;

    public interface IRentalRequestsService
    {
        Task CreateAsync(int eventId, int arenaId);

        Task SetPassedStatusAsync(int arenaId);

        Task<EventInfoViewModel> GetDetails(string arenaRentalRequestId);

        Task ChangeStatus(string id, ArenaRentalRequestStatus approved);
    }
}
