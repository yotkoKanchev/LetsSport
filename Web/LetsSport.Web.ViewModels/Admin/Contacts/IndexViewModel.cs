namespace LetsSport.Web.ViewModels.Admin.Contacts
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<ContactInfoViewModel> Messages { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public int ResultCount { get; set; }
    }
}
