namespace LetsSport.Data.Models.ArenaModels
{
    using System.ComponentModel.DataAnnotations;

    public enum ArenaRentalRequestStatus
    {
        [Display(Name = "Not Approved")]
        NotApproved = 1,
        Approved = 2,
        Denied = 3,
        Passed = 4,
        Canceled = 5,
    }
}
