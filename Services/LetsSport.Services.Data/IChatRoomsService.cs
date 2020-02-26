namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    public interface IChatRoomsService
    {
        Task CreateAsync(int eventId, string userId);
    }
}
