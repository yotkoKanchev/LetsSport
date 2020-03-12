namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
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

        public string Email { get; set; }

        public string Address { get; set; }

        [DisplayName("Administrator")]
        public string ArenaAdminUserName { get; set; }

        public string MainImageUrl { get; set; }

        public IEnumerable<string> Pictures { get; set; }

        public double Rating { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, ArenaDetailsViewModel>()
                  .ForMember(vm => vm.Address, opt => opt.MapFrom(a => a.Address.StreetAddress + ", " + a.Address.City.Name + ", " + a.Address.City.Country.Name))
                  .ForMember(vm => vm.Rating, opt => opt.MapFrom(a => (double)a.Events.Count));
        }
    }
}
