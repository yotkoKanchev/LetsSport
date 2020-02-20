namespace LetsSport.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.ChatModels;

    public class ChatRoomsService : IChatRoomsService
    {
        private readonly ApplicationDbContext db;

        public ChatRoomsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<string> Create()
        {
            var chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.UtcNow,
            };

            await this.db.ChatRooms.AddAsync(chatRoom);
            await this.db.SaveChangesAsync();

            return chatRoom.Id;
        }
    }
}
