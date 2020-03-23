namespace LetsSport.Web.ViewModels.Arenas
{
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class ArenaToSelectListItemViewModel : IMapFrom<Arena>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
