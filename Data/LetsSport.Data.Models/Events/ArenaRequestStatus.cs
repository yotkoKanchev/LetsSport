namespace LetsSport.Data.Models.Events
{
    using System.ComponentModel.DataAnnotations;

    public enum ArenaRequestStatus
    {
        [Display(Name = "Not Sent")]
        NotSent = 1,
        Sent = 2,
        Approved = 3,
        Denied = 4,
    }
}
