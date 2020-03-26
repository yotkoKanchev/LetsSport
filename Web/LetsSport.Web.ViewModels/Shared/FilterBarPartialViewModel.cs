namespace LetsSport.Web.ViewModels.Shared
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FilterBarPartialViewModel
    {
        public int? City { get; set; }

        public int? Sport { get; set; }

        // public string Country { get; set; }
        [DataType(DataType.Date)]
        public DateTime From { get; set; }

        [DataType(DataType.Date)]
        public DateTime To { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }
}
