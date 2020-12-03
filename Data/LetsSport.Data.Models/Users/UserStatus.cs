namespace LetsSport.Data.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public enum UserStatus
    {
        [Display(Name = "Proposal Open")]
        ProposalOpen = 1,

        [Display(Name = "Proposal Colsed")]
        ProposalClosed = 2,
        Unavailable = 3,
    }
}
