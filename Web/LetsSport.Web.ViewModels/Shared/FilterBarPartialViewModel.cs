namespace LetsSport.Web.ViewModels.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FilterBarPartialViewModel
    {
        public string City { get; set; }

        public string Country { get; set; }

        public string Sport { get; set; }

        [DataType(DataType.Date)]
        public DateTime From { get; set; }

        [DataType(DataType.Date)]
        public DateTime To { get; set; }

        public IEnumerable<string> Cities { get; set; }

        public IEnumerable<string> Sports { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }
}
