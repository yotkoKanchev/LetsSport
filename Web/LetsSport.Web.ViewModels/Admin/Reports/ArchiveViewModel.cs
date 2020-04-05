namespace LetsSport.Web.ViewModels.Admin.Reports
{
    using System;

    using LetsSport.Data.Models;

    public class ArchiveViewModel
    {
        public int Id { get; set; }

        public string SenderUserName { get; set; }

        public string ReportedUserUserName { get; set; }

        public AbuseType Abuse { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
