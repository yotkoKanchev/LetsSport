namespace LetsSport.Web.ViewModels.Reports
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;

    public class ReportInputModel
    {
        [Required]
        public string SenderUserId { get; set; }

        public string SenderUserName { get; set; }

        [Required]
        public string ReportedUserId { get; set; }

        public string ReportedUserUserName { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; }

        [Display(Name = "Abusing Type")]
        public AbuseType Abuse { get; set; }
    }
}
