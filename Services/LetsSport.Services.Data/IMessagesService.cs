namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    public interface IMessagesService
    {
        Task CreateMessageAsync(string messageText, string userId, int eventId);

        Task AddInitialMessageAsync(string userId, int eventId);
    }
}
