﻿namespace LetsSport.Services.Data
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;

    public interface IRentalRequestsService
    {
        Task CreateAsync(int eventId, int arenaId);

        Task ChangeStatusAsync(string id, ArenaRentalRequestStatus approved);
    }
}
