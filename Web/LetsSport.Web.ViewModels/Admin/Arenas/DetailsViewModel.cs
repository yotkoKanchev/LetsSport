namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using LetsSport.Data.Models.Arenas;
    using LetsSport.Services.Mapping;

    public class DetailsViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportName { get; set; }

        public string CountryName { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string WebUrl { get; set; }

        public string Email { get; set; }

        public double PricePerHour { get; set; }

        public ArenaStatus Status { get; set; }

        public string Description { get; set; }

        public string AdminName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string MainImageUrl { get; set; }

        public IList<string> ImagesUrls { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, DetailsViewModel>()
                .ForMember(a => a.AdminName, opt => opt.MapFrom(a => a.ArenaAdmin.FirstName + " " + a.ArenaAdmin.LastName))
                .ForMember(a => a.ImagesUrls, opt => opt.MapFrom(a => a.Images.Select(i => i.Url).ToList()));
        }
    }
}
