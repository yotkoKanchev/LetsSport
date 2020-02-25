namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ChatModels;

    public class ChatRoomsService : IChatRoomsService
    {
        private readonly IRepository<ChatRoom> chatRoomsRepository;

        public ChatRoomsService(IRepository<ChatRoom> chatRoomsRepository)
        {
            this.chatRoomsRepository = chatRoomsRepository;
        }

        public async Task<string> CreateAsync()
        {
            var chatRoom = new ChatRoom();

            await this.chatRoomsRepository.AddAsync(chatRoom);
            await this.chatRoomsRepository.SaveChangesAsync();

            return chatRoom.Id;
        }
    }
}
