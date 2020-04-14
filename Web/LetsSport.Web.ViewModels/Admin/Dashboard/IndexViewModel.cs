namespace LetsSport.Web.ViewModels.Administration.Dashboard
{
    using System.ComponentModel.DataAnnotations;

    public class IndexViewModel
    {
        [Display(Name = "Users: ")]
        public int UsersCount { get; set; }

        [Display(Name = "Countries: ")]
        public int CountriesCount { get; set; }

        [Display(Name = "Cities: ")]
        public int CitiesCount { get; set; }

        [Display(Name = "Events: ")]
        public int EventsCount { get; set; }

        [Display(Name = "Arenas: ")]
        public int ArenasCount { get; set; }

        [Display(Name = "Sports: ")]
        public int SportsCount { get; set; }

        [Display(Name = "Reports: ")]
        public int ReportsCount { get; set; }
    }
}
