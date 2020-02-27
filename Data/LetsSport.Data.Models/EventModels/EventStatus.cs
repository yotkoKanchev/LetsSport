namespace LetsSport.Data.Models.EventModels
{
    using System.ComponentModel;

    public enum EventStatus
    {
        Unknown = 0,
        [Description("Accepting Players")]
        AcceptingPlayers = 1,
        [Description("Minimum Players Reached")]
        MinimumPlayersReached = 2,
        Full = 3,
        InProgress = 4,
        Finished = 5,
        Failed = 6,
    }
}
