namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Data.Common;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class ArenaDetailsViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportName { get; set; }

        [DisplayName("Price per Hour")]
        public double PricePerHour { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web-address")]
        public string WebUrl { get; set; }

        // public string Description { get; set; }
        // public string Email { get; set; }
        public string Address { get; set; }

        [DisplayName("Administrator")]
        public string ArenaAdminUserName { get; set; }

        public string ArenaAdminId { get; set; }

        public string MainImageUrl { get; set; }

        public string Status { get; set; }

        public IEnumerable<string> Pictures { get; set; }

        public int EventsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, ArenaDetailsViewModel>()
                  .ForMember(vm => vm.Status, opt => opt.MapFrom(a => a.Status.GetDisplayName()));
        }
    }
}
