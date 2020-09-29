namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models.Mappings;
    using Microsoft.EntityFrameworkCore;

    public class EventUsersSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.EventsUsers.Any())
            {
                return;
            }

            var random = new Random();
            var eventUsers = new List<EventUser>();
            var eventsCount = dbContext.Events.Count();
            var adminRoleId = await dbContext.Roles
                .Where(r => r.Name == GlobalConstants.ArenaAdminRoleName)
                .Select(r => r.Id)
                .FirstAsync();

            var singleAdminCities = await dbContext.Cities
                                        .Where(x => x.Name == "Burgas" || x.Name == "Varna" || x.Name == "Ruse")
                                        .Select(c => c.Id)
                                        .ToListAsync();

            foreach (var cityId in singleAdminCities)
            {
                for (int i = 0; i < 3; i++)
                {
                    var userId = await dbContext.ApplicationUsers
                        .Where(au => au.CityId == cityId)
                        .Where(au => au.Roles
                            .Any(r => r.RoleId == adminRoleId))
                        .Select(au => au.Id)
                        .FirstAsync();

                    var eventId = await dbContext.Events.Where(e => e.CityId == cityId).Select(e => e.Id).Skip(i).FirstAsync();

                    var eventUser = new EventUser
                    {
                        EventId = eventId,
                        UserId = userId,
                    };

                    eventUsers.Add(eventUser);
                }
            }

            var plovdivId = await dbContext.Cities
                                        .Where(x => x.Name == "Plovdiv")
                                        .Select(c => c.Id)
                                        .FirstAsync();

            var plovdivEvents = await dbContext.Events.Where(e => e.CityId == plovdivId).Select(e => e.Id).ToListAsync();

            for (int i = 1; i <= 6; i++)
            {
                var userId = await dbContext.ApplicationUsers
                        .Where(au => au.CityId == plovdivId)
                        .Where(au => au.Roles
                            .Any(r => r.RoleId == adminRoleId))
                        .Select(au => au.Id)
                        .Skip(i % 2)
                        .FirstAsync();

                var eventUser = new EventUser
                {
                    EventId = plovdivEvents[i - 1],
                    UserId = userId,
                };

                eventUsers.Add(eventUser);
            }

            var sofiaId = await dbContext.Cities
                                        .Where(x => x.Name == "Sofia")
                                        .Select(c => c.Id)
                                        .FirstAsync();

            var sofiaEvents = await dbContext.Events.Where(e => e.CityId == sofiaId).Select(e => e.Id).ToListAsync();
            var sofiaUsers = await dbContext.ApplicationUsers
                        .Where(au => au.CityId == sofiaId)
                        .Where(au => au.Roles
                            .Any(r => r.RoleId == adminRoleId))
                        .Select(au => au.Id)
                        .ToListAsync();

            for (int i = 1; i <= 10; i++)
            {
                var eventUser = new EventUser
                {
                    EventId = sofiaEvents[i - 1],
                    UserId = sofiaUsers[i > 5 ? i - 6 : i - 1],
                };

                eventUsers.Add(eventUser);
            }

            dbContext.EventsUsers.AddRange(eventUsers);
        }
    }
}
