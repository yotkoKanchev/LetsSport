namespace LetsSport.Web.ViewModels.Admin.Reports
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class DetailsViewModel : IMapFrom<Report>
    {
        public int Id { get; set; }

        public string SenderUserName { get; set; }

        public string SenderId { get; set; }

        public string ReportedUserUserName { get; set; }

        public string ReportedUserId { get; set; }

        public AbuseType Abuse { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
