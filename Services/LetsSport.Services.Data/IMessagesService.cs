namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    public interface IMessagesService
    {
        Task CreateMessageAsync(string messageText, string userId, string chatRoomId);

        Task AddInitialMessageAsync(string userId, string chatRoomId);
    }
}
