namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ChatModels;

    public class MessagesService : IMessagesService
    {
        private readonly IRepository<Message> messagesRepository;

        public MessagesService(IRepository<Message> messagesRepository)
        {
            this.messagesRepository = messagesRepository;
        }

        public async Task AddInitialMessageAsync(string userId, string chatRoomId)
        {
            var initialMessageText = "Welcome to our new sport event!";

            await this.CreateMessageAsync(initialMessageText, userId, chatRoomId);
        }

        public async Task CreateMessageAsync(string messageText, string userId, string chatRoomId)
        {
            var message = new Message
            {
                ChatRoomId = chatRoomId,
                Text = messageText,
                SenderId = userId,
            };

            await this.messagesRepository.AddAsync(message);
            await this.messagesRepository.SaveChangesAsync();
        }
    }
}
