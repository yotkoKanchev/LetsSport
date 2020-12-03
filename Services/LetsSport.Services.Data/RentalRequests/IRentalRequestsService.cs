namespace LetsSport.Services.Data.RentalRequests
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.Arenas;

    public interface IRentalRequestsService
    {
        Task CreateAsync(int eventId, int arenaId);

        Task ChangeStatusAsync(string id, ArenaRentalRequestStatus approved);
    }
}
