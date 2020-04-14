namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using Microsoft.EntityFrameworkCore;

    public class ArenaRequestsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.ArenaRentalRequests.Any())
            {
                return;
            }

            var requests = new List<ArenaRentalRequest>();

            for (int i = 1; i < 10; i++)
            {
                var eventId = await dbContext.Events.Where(e => e.ArenaId == i).Select(e => e.Id).FirstAsync();
                var request = new ArenaRentalRequest
                {
                    ArenaId = i,
                    EventId = eventId,
                    Status = (ArenaRentalRequestStatus)1,
                };

                requests.Add(request);

                var evt = await dbContext.Events.Where(e => e.Id == eventId).FirstAsync();
                evt.RequestStatus = (ArenaRequestStatus)2;
                dbContext.Events.Update(evt);
            }

            dbContext.ArenaRentalRequests.AddRange(requests);
        }
    }
}
