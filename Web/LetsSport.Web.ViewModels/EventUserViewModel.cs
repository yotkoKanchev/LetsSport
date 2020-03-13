namespace LetsSport.Web.ViewModels
{
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Services.Mapping;

    public class EventUserViewModel : IMapTo<EventUser>, IMapFrom<EventUser>
    {
        public string UserUserProfileId { get; set; }

        public string UserUserName { get; set; }
    }
}
