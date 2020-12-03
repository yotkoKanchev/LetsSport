namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.Arenas;
    using LetsSport.Data.Models.Events;
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
            var arenaIds = await dbContext.Arenas.Select(a => a.Id).ToListAsync();

            foreach (var id in arenaIds)
            {
                var eventId = await dbContext.Events.Where(e => e.ArenaId == id).Select(e => e.Id).FirstOrDefaultAsync();

                if (eventId != 0)
                {
                    var request = new ArenaRentalRequest
                    {
                        ArenaId = id,
                        EventId = eventId,
                        Status = (ArenaRentalRequestStatus)1,
                    };

                    requests.Add(request);

                    var evt = await dbContext.Events.Where(e => e.Id == eventId).FirstAsync();
                    evt.RequestStatus = (ArenaRequestStatus)2;
                    dbContext.Events.Update(evt);
                }
            }

            dbContext.ArenaRentalRequests.AddRange(requests);
        }
    }
}
