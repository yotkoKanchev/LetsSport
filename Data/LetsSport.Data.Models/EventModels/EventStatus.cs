﻿namespace LetsSport.Data.Models.EventModels
{
    public enum EventStatus
    {
        Unknown = 0,
        AcceptingPlayers = 1,
        MinimumPlayersReached = 2,
        Full = 3,
        InProgress = 4,
        Finished = 5,
        Failed = 6,
    }
}