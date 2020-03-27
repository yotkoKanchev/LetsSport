namespace LetsSport.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class FilterBarPartialViewModel
    {
        public int? City { get; set; }

        public int? Sport { get; set; }

        [DataType(DataType.Date)]
        public DateTime From { get; set; }

        [DataType(DataType.Date)]
        public DateTime To { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; }
    }
}
