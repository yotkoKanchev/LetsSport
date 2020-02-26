namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Web.ViewModels.Messages;

    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> messagesRepository;

        public MessageService(IRepository<Message> messagesRepository)
        {
            this.messagesRepository = messagesRepository;
        }

        public async Task Create(MessageCreateInputModel inputModel, string userId, string chatRoomId)
        {
            var message = new Message
            {
                ChatRoomId = chatRoomId,
                Text = inputModel.MessageText,
                SenderId = userId,
            };

            await this.messagesRepository.AddAsync(message);
        }
    }
}
