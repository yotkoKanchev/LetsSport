namespace LetsSport.Data.Models.Events
{
    using System.ComponentModel.DataAnnotations;

    public enum EventStatus
    {
        [Display(Name = "Accepting Players")]
        AcceptingPlayers = 1,

        [Display(Name = "Minimum Players Reached")]
        MinimumPlayersReached = 2,

        Full = 3,
        Failed = 4,
        Canceled = 5,
        Passed = 6,
    }
}
