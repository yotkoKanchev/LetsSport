namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    public interface IMessagesService
    {
        Task Create(string messageText, string userId, string chatRoomId);

        Task AddInitialMessageAsync(string userId, string chatRoomId);
    }
}
