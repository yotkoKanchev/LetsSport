namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public string Location { get; set; }

        public int CountryId { get; set; }

        public IEnumerable<InfoViewModel> Events { get; set; }

        public FilterBarViewModel Filter { get; set; }

        public int? CityId { get; set; }

        public int? SportId { get; set; }

        public int? IsDeleted { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public int ResultCount { get; set; }
    }
}
