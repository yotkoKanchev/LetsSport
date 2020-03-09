namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;

    public class MessagesService : IMessagesService
    {
        private readonly IRepository<Message> messagesRepository;

        public MessagesService(IRepository<Message> messagesRepository)
        {
            this.messagesRepository = messagesRepository;
        }

        public async Task AddInitialMessageAsync(string userId, int eventId)
        {
            var initialMessageText = "Welcome to our new sport event!";

            await this.CreateMessageAsync(initialMessageText, userId, eventId);
        }

        public async Task CreateMessageAsync(string messageText, string userId, int eventId)
        {
            var message = new Message
            {
                EventId = eventId,
                Content = messageText,
                SenderId = userId,
            };

            await this.messagesRepository.AddAsync(message);
            await this.messagesRepository.SaveChangesAsync();
        }
    }
}
