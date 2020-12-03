namespace LetsSport.Web.ViewModels.Arenas
{
    using AutoMapper;
    using LetsSport.Common;
    using LetsSport.Data.Models.Arenas;
    using LetsSport.Services.Mapping;

    public class ArenaIndexInfoViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AddressStreetAddress { get; set; }

        public string SportName { get; set; }

        public double PricePerHour { get; set; }

        public string MainImageUrl { get; set; }

        public int EventsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, ArenaIndexInfoViewModel>()
                .ForMember(a => a.MainImageUrl, opt => opt.MapFrom(a => GlobalConstants.CardImageSizing + a.MainImage.Url));
        }
    }
}
