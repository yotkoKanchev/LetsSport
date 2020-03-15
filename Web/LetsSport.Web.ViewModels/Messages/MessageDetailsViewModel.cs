namespace LetsSport.Web.ViewModels.Messages
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class MessageDetailsViewModel : IMapFrom<Message>, IMapTo<Message>
    {
        public string Id { get; set; }

        public string SenderUserName { get; set; }

        public string SenderId { get; set; }

        public string SenderUserProfileAvatarUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }
    }
}
