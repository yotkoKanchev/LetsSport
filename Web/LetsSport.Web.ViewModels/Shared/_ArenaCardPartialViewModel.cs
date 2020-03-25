namespace LetsSport.Web.ViewModels.Shared
{
    using AutoMapper;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;

    public class _ArenaCardPartialViewModel : IMapFrom<Arena>, IHaveCustomMappings
    {
        private readonly string detailsImageSizing = "w_384,h_256,c_scale,r_10,bo_3px_solid_silver/";

        public int Id { get; set; }

        public string Name { get; set; }

        public string AddressStreetAddress { get; set; }

        public string SportName { get; set; }

        public double PricePerHour { get; set; }

        public string MainImageUrl { get; set; }

        public int EventsCount { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Arena, _ArenaCardPartialViewModel>()
                .ForMember(a => a.MainImageUrl, opt => opt.MapFrom(a => this.detailsImageSizing + a.MainImage.Url));
        }
    }
}
