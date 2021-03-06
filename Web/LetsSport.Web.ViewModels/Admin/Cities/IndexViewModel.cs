﻿namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<CityInfoViewModel> Cities { get; set; }

        public string Location { get; set; }

        public SimpleModelsFilterBarViewModel Filter { get; set; }

        public int CountryId { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public int ResultCount { get; set; }

        public int DeletionStatus { get; set; }
    }
}
