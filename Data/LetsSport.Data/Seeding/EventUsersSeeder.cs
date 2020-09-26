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
                .Where(r => r.Name == GlobalConstants.AdministratorRoleName)
                .Select(r => r.Id)
                .FirstAsync();

            for (int i = 1; i < eventsCount; i++)
            {
                var cityId = await dbContext.Events.Where(e => e.Id == i).Select(e => e.CityId).FirstAsync();

                for (int j = 0; j < random.Next(1, 3); j++)
                {
                    var eventUser = new EventUser
                    {
                        EventId = i,
                        UserId = await dbContext.ApplicationUsers
                        .Where(au => au.CityId == cityId)
                        .Where(au => au.Roles
                            .Any(r => r.RoleId == adminRoleId))
                        .Select(au => au.Id)
                        .Skip(j % 2)
                        .FirstAsync(),
                    };

                    eventUsers.Add(eventUser);
                }
            }

            dbContext.EventsUsers.AddRange(eventUsers);
        }
    }
}
