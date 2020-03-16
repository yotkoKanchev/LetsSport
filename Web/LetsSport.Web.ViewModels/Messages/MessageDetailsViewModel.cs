namespace LetsSport.Web.ViewModels.Messages
{
    using System;

    using AutoMapper;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class MessageDetailsViewModel : IMapFrom<Message>, IMapTo<Message>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string SenderUserName { get; set; }

        public string SenderId { get; set; }

        public string SenderAvatarUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Content { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Message, MessageDetailsViewModel>()
                .ForMember(m => m.CreatedOn, opt => opt.MapFrom(m => m.CreatedOn.ToLocalTime()));
        }
    }
}
