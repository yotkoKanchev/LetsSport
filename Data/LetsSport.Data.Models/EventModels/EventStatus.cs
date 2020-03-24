namespace LetsSport.Data.Models.EventModels
{
    using System.ComponentModel.DataAnnotations;

    public enum EventStatus
    {
        [Display(Name = "Accepting Players")]
        AcceptingPlayers = 1,

        [Display(Name = "Minimum Players Reached")]
        MinimumPlayersReached = 2,

        Full = 3,
        Finished = 4,
        Failed = 5,
        Passed = 6,
        Canceled = 7,
    }
}
