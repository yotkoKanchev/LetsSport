namespace LetsSport.Web.ViewModels.EventsUsers
{
    using AutoMapper;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Mapping;

    public class EventUserViewModel : IMapTo<EventUser>, IMapFrom<EventUser>, IHaveCustomMappings
    {
        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public string UserScore { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<EventUser, EventUserViewModel>()
                .ForMember(vm => vm.UserScore, opt => opt.MapFrom(e => $"{e.User.Events.Count}/{e.User.AdministratingEvents.Count}"));
        }
    }
}
