namespace LetsSport.Web.ViewModels.Admin.Reports
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class ReportInfoViewModel : IMapFrom<Report>
    {
        public int Id { get; set; }

        // public string SenderCountryName { get; set; }
        public int SenderCountryId { get; set; }

        public string SenderUserName { get; set; }

        public string ReportedUserUserName { get; set; }

        public AbuseType Abuse { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
