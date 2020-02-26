namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ChatModels;

    public class ChatRoomsService : IChatRoomsService
    {
        private readonly IRepository<ChatRoom> chatRoomsRepository;
        private readonly IMessagesService messageService;

        public ChatRoomsService(IRepository<ChatRoom> chatRoomsRepository, IMessagesService messageService)
        {
            this.chatRoomsRepository = chatRoomsRepository;
            this.messageService = messageService;
        }

        public async Task CreateAsync(int eventId, string userId)
        {
            var chatRoom = new ChatRoom
            {
                EventId = eventId,
            };

            await this.chatRoomsRepository.AddAsync(chatRoom);
            await this.chatRoomsRepository.SaveChangesAsync();

            // TODO Can be removed!
            await this.messageService.AddInitialMessageAsync(userId, chatRoom.Id);
        }
    }
}
