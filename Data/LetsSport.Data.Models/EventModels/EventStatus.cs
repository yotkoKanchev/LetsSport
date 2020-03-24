namespace LetsSport.Data.Models.EventModels
{
    using System.ComponentModel;

    public enum EventStatus
    {
        [Description("Accepting Players")]
        AcceptingPlayers = 1,
        [Description("Minimum Players Reached")]
        MinimumPlayersReached = 2,
        Full = 3,
        Finished = 4,
        Failed = 5,
        Passed = 6,
        Canceled = 7,
    }
}
