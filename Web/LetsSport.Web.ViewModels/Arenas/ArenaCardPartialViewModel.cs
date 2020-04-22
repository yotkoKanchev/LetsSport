namespace LetsSport.Web.ViewModels.Arenas
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using LetsSport.Common;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class ArenaCardPartialViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int SportId { get; set; }

        public int CityId { get; set; }

        public string SportName { get; set; }

        [Display(Name = "Price per hour:")]
        public double PricePerHour { get; set; }

        public string MainImageUrl { get; set; }

        [Display(Name = "Orginized events:")]
        public int EventsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, ArenaCardPartialViewModel>()
                .ForMember(a => a.MainImageUrl, opt => opt.MapFrom(a => GlobalConstants.CardImageSizing + a.MainImage.Url));
        }
    }
}
