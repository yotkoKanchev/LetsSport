namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenaEditViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string ArenaAdminId { get; set; }

        public string Name { get; set; }

        [DisplayName("Sport Type")]

        public int SportId { get; set; }

        [DisplayName("Price per Hour")]

        public double PricePerHour { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web address")]
        public string WebUrl { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        [DisplayName("Street Address")]
        public string AddressStreetAddress { get; set; }

        public string AddressCityCountryName { get; set; }

        public string AddressCityName { get; set; }

        public string Address { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, ArenaEditViewModel>()
                .ForMember(vm => vm.Address, opt => opt.MapFrom(a => a.Address.StreetAddress + ", " + a.Address.City.Name + ", " + a.Address.City.Country.Name));
        }
    }
}
