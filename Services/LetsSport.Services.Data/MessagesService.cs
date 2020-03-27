namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.Extensions.Configuration;

    public class MessagesService : IMessagesService
    {
        private const string InvalidMessageIdErrorMessage = "Message with ID: {0} does not exist.";
        private const string NoAvatarImagePath = "../../images/noAvatar.png";
        private readonly IRepository<Message> messagesRepository;
        private readonly IConfiguration configuration;
        private readonly string imagePathPrefix;

        public MessagesService(IRepository<Message> messagesRepository, IConfiguration configuration)
        {
            this.messagesRepository = messagesRepository;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format("https://res.cloudinary.com/{0}/image/upload/", this.configuration["Cloudinary:ApiName"]);
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

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidMessageIdErrorMessage, id));
            }

            var messages = query.To<MessageDetailsViewModel>();

            var messagesList = messages.ToList();

            foreach (var message in messagesList)
            {
                if (message.SenderAvatarUrl == null)
                {
                    message.SenderAvatarUrl = NoAvatarImagePath;
                }
                else
                {
                    message.SenderAvatarUrl = this.imagePathPrefix + message.SenderAvatarUrl;
                }
            }

            return messagesList;
        }
    }
}
