namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Messages;

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

        public IEnumerable<MessageDetailsViewModel> GetMessagesByEventId(int id)
        {
            var query = this.messagesRepository.All()
                .Where(m => m.EventId == id)
                .OrderByDescending(m => m.CreatedOn);

            var messages = query.To<MessageDetailsViewModel>();

            return messages.ToList();
        }
    }
}
