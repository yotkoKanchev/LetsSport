namespace LetsSport.Services.Data.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Cloudinary;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;
    using static LetsSport.Common.GlobalConstants;

    public class MessagesService : IMessagesService
    {
        private readonly IRepository<Message> messagesRepository;
        private readonly string imagePathPrefix;

        public MessagesService(IRepository<Message> messagesRepository, ICloudinaryHelper cloudinaryHelper)
        {
            this.messagesRepository = messagesRepository;
            this.imagePathPrefix = cloudinaryHelper.GetPrefix();
        }

        public async Task<string> CreateAsync(string content, string userId, int eventId)
        {
            var message = new Message
            {
                EventId = eventId,
                Content = content,
                SenderId = userId,
            };

            await this.messagesRepository.AddAsync(message);
            await this.messagesRepository.SaveChangesAsync();

            return message.Id;
        }

        public async Task<IEnumerable<MessageDetailsViewModel>> GetAllByEventIdAsync(int id)
        {
            var query = this.messagesRepository.All()
                .Where(m => m.EventId == id)
                .OrderByDescending(m => m.CreatedOn);

            if (!query.Any())
            {
                throw new ArgumentNullException(string.Format(MessageInvalidIdErrorMessage, id));
            }

            var messages = await query.To<MessageDetailsViewModel>().ToListAsync();

            foreach (var message in messages)
            {
                if (message.SenderAvatarUrl == null)
                {
                    message.SenderAvatarUrl = NoAvatarImagePath;
                }
                else
                {
                    message.SenderAvatarUrl = this.imagePathPrefix + AvatarImageSizing + message.SenderAvatarUrl;
                }
            }

            return messages;
        }

        public async Task<MessageDetailsViewModel> GetDetailsById(string id)
        {
            var message = await this.messagesRepository
                .All()
                .Where(m => m.Id == id)
                .To<MessageDetailsViewModel>()
                .FirstOrDefaultAsync();
            if (message == null)
            {
                throw new ArgumentException(string.Format(MessageInvalidIdErrorMessage, id));
            }

            if (message.SenderAvatarUrl == null)
            {
                message.SenderAvatarUrl = NoAvatarImagePath;
            }
            else
            {
                message.SenderAvatarUrl = this.imagePathPrefix + AvatarImageSizing + message.SenderAvatarUrl;
            }

            return message;
        }
    }
}
