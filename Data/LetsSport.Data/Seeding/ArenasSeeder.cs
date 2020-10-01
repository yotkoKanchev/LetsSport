namespace LetsSport.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.GlobalConstants;

    public class ArenasSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Arenas.Any())
            {
                return;
            }

            var bulgariaId = await dbContext.Countries.Where(c => c.Name == "Bulgaria").Select(c => c.Id).FirstAsync();

            // Sofia arenas(5)
            var sofiaId = await dbContext.Cities.Where(c => c.Name == "Sofia").Select(c => c.Id).FirstAsync();
            var arenaAdminRoleId = await dbContext.Roles.Where(r => r.Name == ArenaAdminRoleName).Select(r => r.Id).FirstAsync();

            var arenaArmeets = new Arena
            {
                Name = "Arena Armeets",
                Email = "bookings@arena-armeets.bg",
                WebUrl = "www.arena-armeets.bg",
                PhoneNumber = "0878112233",
                SportId = await dbContext.Sports.Where(s => s.Name == "Basketball").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == sofiaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).FirstAsync(),
                Address = "Chetvarti Kilometar",
                Description = "Indoor sports",
                PricePerHour = 1200,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = sofiaId,
            };

            var vasilLevski = new Arena
            {
                Name = "Vasil Levski",
                Email = "bookings@national-stadium.bg",
                WebUrl = "www.national-stadium.bg",
                PhoneNumber = "0888226677",
                SportId = await dbContext.Sports.Where(s => s.Name == "Football").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == sofiaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).Skip(1).FirstAsync(),
                Address = "Evlogi Georgiev blvd.",
                Description = "Outdoor sports",
                PricePerHour = 1400,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = sofiaId,
            };

            var diana = new Arena
            {
                Name = "Diana Pool",
                Email = "bookings@diana-pool.bg",
                WebUrl = "www.diana-pool.bg",
                PhoneNumber = "0886998822",
                SportId = await dbContext.Sports.Where(s => s.Name == "Aquatics").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == sofiaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).Skip(2).FirstAsync(),
                Address = "1 Tintyava str",
                Description = "Indoor pools",
                PricePerHour = 25,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = sofiaId,
            };

            var universiada = new Arena
            {
                Name = "Universiada",
                Email = "bookings@universiada.bg",
                WebUrl = "www.universiada.bg",
                PhoneNumber = "0886998866",
                SportId = await dbContext.Sports.Where(s => s.Name == "Volleyball").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == sofiaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).Skip(3).FirstAsync(),
                Address = "1 Shipchenski prohod blvd.",
                Description = "Indoor sports",
                PricePerHour = 600,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = sofiaId,
            };

            var triaditsa = new Arena
            {
                Name = "Triaditsa",
                Email = "bookings@triaditsa.bg",
                WebUrl = "www.triaditsa.bg",
                PhoneNumber = "0886123456",
                SportId = await dbContext.Sports.Where(s => s.Name == "Boxing").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == sofiaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).Skip(4).FirstAsync(),
                Address = "Gotse Delchev",
                Description = "Indoor sports",
                PricePerHour = 400,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = sofiaId,
            };

            // Plovdiv (2)
            var polovdivId = await dbContext.Cities.Where(c => c.Name == "Plovdiv").Select(c => c.Id).FirstAsync();
            var kolodruma = new Arena
            {
                Name = "Kolodruma",
                Email = "bookings@kolodruma.bg",
                WebUrl = "www.kolodruma.bg",
                PhoneNumber = "0883445566",
                SportId = await dbContext.Sports.Where(s => s.Name == "Basketball").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == polovdivId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).FirstAsync(),
                Address = "Asenovgradsko Shose",
                Description = "Indoor sports",
                PricePerHour = 800,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = polovdivId,
            };

            var sila = new Arena
            {
                Name = "Sila",
                Email = "bookings@sila.bg",
                WebUrl = "www.sila.bg",
                PhoneNumber = "0883324354",
                SportId = await dbContext.Sports.Where(s => s.Name == "Volleyball").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == polovdivId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).Skip(1).FirstAsync(),
                Address = "Vasil Aprilov blvd.",
                Description = "Indoor sports",
                PricePerHour = 500,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = polovdivId,
            };

            // Varna
            var varnaId = await dbContext.Cities.Where(c => c.Name == "Varna").Select(c => c.Id).FirstAsync();

            var sportPalace = new Arena
            {
                Name = "Dvorets na sporta",
                Email = "bookings@sport-palace.bg",
                WebUrl = "www.sport-palace.bg",
                PhoneNumber = "0888442233",
                SportId = await dbContext.Sports.Where(s => s.Name == "Volleyball").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == varnaId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).FirstAsync(),
                Address = "Knyaz Boris I blvd.",
                Description = "Indoor sports",
                PricePerHour = 800,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = varnaId,
            };

            // Burgas
            var burgasId = await dbContext.Cities.Where(c => c.Name == "Burgas").Select(c => c.Id).FirstAsync();

            var burgas = new Arena
            {
                Name = "Burgas",
                Email = "bookings@burgas-stadium.bg",
                WebUrl = "www.burgas-stadium.bg",
                PhoneNumber = "0884556688",
                SportId = await dbContext.Sports.Where(s => s.Name == "Football").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == burgasId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).FirstAsync(),
                Address = "Lazur kv.",
                Description = "Outdoor sports",
                PricePerHour = 600,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = burgasId,
            };

            // Ruse
            var rusesId = await dbContext.Cities.Where(c => c.Name == "Ruse").Select(c => c.Id).FirstAsync();

            var ozk = new Arena
            {
                Name = "OZK",
                Email = "bookings@ozk-arena.bg",
                WebUrl = "www.ozk-arena.bg",
                PhoneNumber = "0887889922",
                SportId = await dbContext.Sports.Where(s => s.Name == "Tennis").Select(s => s.Id).FirstAsync(),
                ArenaAdminId = await dbContext.ApplicationUsers.Where(au => au.CityId == rusesId)
                                                               .Where(au => au.Roles
                                                                              .Any(r => r.RoleId == arenaAdminRoleId))
                                                               .Select(au => au.Id).FirstAsync(),
                Address = "Lipnik blvd.",
                Description = "Indoor sports",
                PricePerHour = 400,
                Status = (ArenaStatus)1,
                CountryId = bulgariaId,
                CityId = rusesId,
            };

            dbContext.Arenas.AddRange(
                arenaArmeets, vasilLevski, universiada, diana, triaditsa, kolodruma, sila, sportPalace, burgas, ozk);
        }
    }
}
