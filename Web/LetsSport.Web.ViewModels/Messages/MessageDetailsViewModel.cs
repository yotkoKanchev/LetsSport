namespace LetsSport.Web.ViewModels.Messages
{
    using AutoMapper;
    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class MessageDetailsViewModel : IMapFrom<Message>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string SenderUserName { get; set; }

        public string SenderId { get; set; }

        public string SenderAvatarUrl { get; set; }

        public string CreatedOn { get; set; }

        public string Content { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Message, MessageDetailsViewModel>()
                .ForMember(m => m.CreatedOn, opt => opt.MapFrom(m => m.CreatedOn.ToString(GlobalConstants.DefaultDateTimeFormat)));
        }
    }
}
