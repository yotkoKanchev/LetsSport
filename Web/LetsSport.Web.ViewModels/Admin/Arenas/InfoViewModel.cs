namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class InfoViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public int CityId { get; set; }

        public string SportName { get; set; }

        public int SportId { get; set; }
    }
}
