namespace LetsSport.Web.ViewModels.Home
{
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Common;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;

    public class HomeEventInfoViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public int ArenaId { get; set; }

        public string ArenaName { get; set; }

        public string ArenaAddressCityName { get; set; }

        public string SportName { get; set; }

        public string Date { get; set; }

        [DisplayName("Empty spots")]
        public int EmptySpotsLeft { get; set; }

        public string Status { get; set; }

        public string SportImage { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, HomeEventInfoViewModel>()
                .ForMember(vm => vm.EmptySpotsLeft, opt => opt.MapFrom(e => e.MaxPlayers - e.Users.Count))
                .ForMember(vm => vm.Date, opt => opt.MapFrom(e =>
                                                 e.Date.ToString(GlobalConstants.DefaultDateFormat) +
                                                 " at " +
                                                 e.StartingHour.ToString(GlobalConstants.DefaultTimeFormat)));
        }
    }
}
