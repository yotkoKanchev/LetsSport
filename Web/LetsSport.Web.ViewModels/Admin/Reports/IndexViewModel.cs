﻿namespace LetsSport.Web.ViewModels.Admin.Reports
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<ReportInfoInputModel> Reports { get; set; }

        public SimpleModelsFilterBarViewModel Filter { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public int ResultCount { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
