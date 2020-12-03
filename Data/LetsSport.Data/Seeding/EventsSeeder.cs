namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.Events;
    using LetsSport.Data.Models.Users;
    using Microsoft.EntityFrameworkCore;

    public class EventsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Events.Any())
            {
                return;
            }

            var bulgariaId = await dbContext.Countries.Where(c => c.Name == "Bulgaria").Select(c => c.Id).FirstAsync();
            var random = new Random();
            var events = new List<Event>();

            // Sofia(10)
            var sofiaId = await dbContext.Cities.Where(c => c.Name == "Sofia").Select(c => c.Id).FirstAsync();
            for (int i = 0; i < 5; i++)
            {
                var arenaIQueryable = dbContext.Arenas.Where(a => a.CityId == sofiaId).Skip(i);
                var arenaId = await arenaIQueryable.Select(a => a.Id).FirstAsync();
                var sportId = await arenaIQueryable.Select(a => a.SportId).FirstAsync();
                var sportName = await arenaIQueryable.Select(a => a.Sport.Name).FirstAsync();

                for (int j = 1; j <= 2; j++)
                {
                    var evt = new Event
                    {
                        Name = sportName + $" {j}",
                        AdminId = await dbContext.ApplicationUsers
                          .Where(u => u.CityId == sofiaId)
                          .Select(u => u.Id)
                          .Skip(random.Next(0, 5))
                          .FirstAsync(),
                        ArenaId = arenaId,
                        CountryId = bulgariaId,
                        CityId = sofiaId,
                        SportId = sportId,
                        Gender = (Gender)random.Next(1, 4),
                        Date = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                        StartingHour = DateTime.UtcNow.AddHours(random.Next(1, 12)),
                        GameFormat = $"format {i + 1}x{j}",
                        MinPlayers = i + 1 + j,
                        MaxPlayers = i > j & j == 1 ? i + 1 + 1 : (i + 1) * j,
                        AdditionalInfo = "all are welcome to join",
                        DurationInHours = i + 1 - j < 1 ? 1 : i + 1 - j,
                        Status = (EventStatus)1,
                        RequestStatus = (ArenaRequestStatus)1,
                    };

                    events.Add(evt);
                }
            }

            // Plovdiv(6)
            var plovdivId = await dbContext.Cities.Where(c => c.Name == "Plovdiv").Select(c => c.Id).FirstAsync();
            for (int i = 0; i < 2; i++)
            {
                var arenaIQueryable = dbContext.Arenas.Where(a => a.CityId == plovdivId).Skip(i);
                var arenaId = await arenaIQueryable.Select(a => a.Id).FirstAsync();
                var sportId = await arenaIQueryable.Select(a => a.SportId).FirstAsync();
                var sportName = await arenaIQueryable.Select(a => a.Sport.Name).FirstAsync();

                for (int j = 1; j <= 3; j++)
                {
                    var evt = new Event
                    {
                        Name = sportName + $" {j}",
                        AdminId = await dbContext.ApplicationUsers
                          .Where(u => u.CityId == plovdivId)
                          .Select(u => u.Id)
                          .Skip(random.Next(0, 2))
                          .FirstAsync(),
                        ArenaId = arenaId,
                        CountryId = bulgariaId,
                        CityId = plovdivId,
                        SportId = sportId,
                        Gender = (Gender)random.Next(1, 4),
                        Date = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                        StartingHour = DateTime.UtcNow.AddHours(random.Next(1, 12)),
                        GameFormat = $"format {i + 1}x{j}",
                        MinPlayers = i + 3 + j,
                        MaxPlayers = i > j & j == 1 ? i + 3 + 1 : (i + 3) * j,
                        AdditionalInfo = "come join us",
                        DurationInHours = i + 1 - j < 1 ? 1 : i + 1 - j,
                        Status = (EventStatus)1,
                        RequestStatus = (ArenaRequestStatus)1,
                    };

                    events.Add(evt);
                }
            }

            // Varna(3)
            var varnaId = await dbContext.Cities.Where(c => c.Name == "Varna").Select(c => c.Id).FirstAsync();
            for (int i = 1; i <= 3; i++)
            {
                var arenaIQueryable = dbContext.Arenas.Where(a => a.CityId == varnaId);
                var arenaId = await arenaIQueryable.Select(a => a.Id).FirstAsync();
                var sportId = await arenaIQueryable.Select(a => a.SportId).FirstAsync();
                var sportName = await arenaIQueryable.Select(a => a.Sport.Name).FirstAsync();

                var evt = new Event
                {
                    Name = sportName + $" {i}",
                    AdminId = await dbContext.ApplicationUsers
                      .Where(u => u.CityId == varnaId)
                      .Select(u => u.Id)
                      .FirstAsync(),
                    ArenaId = arenaId,
                    CountryId = bulgariaId,
                    CityId = varnaId,
                    SportId = sportId,
                    Gender = (Gender)random.Next(1, 4),
                    Date = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                    StartingHour = DateTime.UtcNow.AddHours(random.Next(1, 12)),
                    GameFormat = $"format {i + 1}x{i + 1}",
                    MinPlayers = i + 3,
                    MaxPlayers = (i + 3) * 5,
                    AdditionalInfo = "come join us",
                    DurationInHours = i + 1,
                    Status = (EventStatus)1,
                    RequestStatus = (ArenaRequestStatus)1,
                };

                events.Add(evt);
            }

            // Burgas(3)
            var burgasId = await dbContext.Cities.Where(c => c.Name == "Burgas").Select(c => c.Id).FirstAsync();
            for (int i = 1; i <= 3; i++)
            {
                var arenaIQueryable = dbContext.Arenas.Where(a => a.CityId == burgasId);
                var arenaId = await arenaIQueryable.Select(a => a.Id).FirstAsync();
                var sportId = await arenaIQueryable.Select(a => a.SportId).FirstAsync();
                var sportName = await arenaIQueryable.Select(a => a.Sport.Name).FirstAsync();

                var evt = new Event
                {
                    Name = sportName + $" {i}",
                    AdminId = await dbContext.ApplicationUsers
                      .Where(u => u.CityId == burgasId)
                      .Select(u => u.Id)
                      .FirstAsync(),
                    ArenaId = arenaId,
                    CountryId = bulgariaId,
                    CityId = burgasId,
                    SportId = sportId,
                    Gender = (Gender)random.Next(1, 4),
                    Date = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                    StartingHour = DateTime.UtcNow.AddHours(random.Next(1, 12)),
                    GameFormat = $"format {i + 1}x{i + 1}",
                    MinPlayers = i + 3,
                    MaxPlayers = (i + 3) * 5,
                    AdditionalInfo = "come join us",
                    DurationInHours = i + 1,
                    Status = (EventStatus)1,
                    RequestStatus = (ArenaRequestStatus)1,
                };

                events.Add(evt);
            }

            // Ruse(3)
            var ruseId = await dbContext.Cities.Where(c => c.Name == "Ruse").Select(c => c.Id).FirstAsync();
            for (int i = 1; i <= 3; i++)
            {
                var arenaIQueryable = dbContext.Arenas.Where(a => a.CityId == ruseId);
                var arenaId = await arenaIQueryable.Select(a => a.Id).FirstAsync();
                var sportId = await arenaIQueryable.Select(a => a.SportId).FirstAsync();
                var sportName = await arenaIQueryable.Select(a => a.Sport.Name).FirstAsync();

                var evt = new Event
                {
                    Name = sportName + $" {i}",
                    AdminId = await dbContext.ApplicationUsers
                      .Where(u => u.CityId == ruseId)
                      .Select(u => u.Id)
                      .FirstAsync(),
                    ArenaId = arenaId,
                    CountryId = bulgariaId,
                    CityId = ruseId,
                    SportId = sportId,
                    Gender = (Gender)random.Next(1, 4),
                    Date = DateTime.UtcNow.AddDays(random.Next(10, 60)),
                    StartingHour = DateTime.UtcNow.AddHours(random.Next(1, 12)),
                    GameFormat = $"format {i + 1}x{i + 1}",
                    MinPlayers = i + 3,
                    MaxPlayers = (i + 3) * 5,
                    AdditionalInfo = "come join us",
                    DurationInHours = i + 1,
                    Status = (EventStatus)1,
                    RequestStatus = (ArenaRequestStatus)1,
                };

                events.Add(evt);
            }

            dbContext.Events.AddRange(events);
        }
    }
}
