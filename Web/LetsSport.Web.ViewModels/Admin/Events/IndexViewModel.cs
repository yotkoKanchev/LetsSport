namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public string Location { get; set; }

        public int CountryId { get; set; }

        public IEnumerable<InfoViewModel> Events { get; set; }

        public FilterBarViewModel Filter { get; set; }
    }
}
