namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using AutoMapper;
    using LetsSport.Data.Common;
    using LetsSport.Data.Models.Arenas;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class MyArenaDetailsViewModel : IMapFrom<Arena>, IHaveCustomMappings
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

        public string Description { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Address { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }

        [DisplayName("Administrator")]
        public string ArenaAdminUserName { get; set; }

        public string ArenaAdminId { get; set; }

        public string MainImageUrl { get; set; }

        public IEnumerable<string> Pictures { get; set; }

        public int EventsCount { get; set; }

        public string LoggedUserId { get; set; }

        [AllowedExtensions]
        [MaxFileSize]
        public IFormFile NewMainImage { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, MyArenaDetailsViewModel>()
                .ForMember(a => a.Status, opt => opt.MapFrom(a => a.Status.GetDisplayName()));
        }
    }
}
