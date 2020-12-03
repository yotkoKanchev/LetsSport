namespace LetsSport.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class UsersSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var bulgariaId = await dbContext.Countries.Where(c => c.Name == "Bulgaria").Select(c => c.Id).FirstAsync();
            var sofiaId = await dbContext.Cities.Where(c => c.Name == "Sofia").Select(c => c.Id).FirstAsync();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            if (dbContext.ApplicationUsers.Any())
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@letssport.com",
                FirstName = "AdminFirstName",
                LastName = "AdminLastName",
                CityId = sofiaId,
                CountryId = bulgariaId,
            };

            var password = "admin123";

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
            }

            var random = new Random();

            // Regular Users(10)
            for (int i = 1; i <= 10; i++)
            {
                var regularUser = new ApplicationUser
                {
                    UserName = $"regular{i}",
                    Email = $"regular{i}@letssport.com",
                    FirstName = $"Regular{i}",
                    LastName = $"Regularov{i}",
                    CountryId = bulgariaId,
                    CityId = i,
                    Age = 18 + i,
                    Gender = (Gender)random.Next(1, 4),
                    SportId = random.Next(1, 31),
                    Status = (UserStatus)random.Next(1, 4),
                };

                var regpass = $"regular{i}";
                await userManager.CreateAsync(regularUser, regpass);
            }

            // Sofia admins (5)
            for (int i = 1; i <= 5; i++)
            {
                var sofiaAdmin = new ApplicationUser
                {
                    UserName = $"sofiaAdmin{i}",
                    Email = $"sofiaAdmin{i}@letssport.com",
                    FirstName = $"Sofian{i}",
                    LastName = $"Sofiev{i}",
                    CountryId = bulgariaId,
                    CityId = sofiaId,
                    Age = 18 + i,
                    Gender = (Gender)random.Next(1, 4),
                    SportId = random.Next(1, 31),
                    Status = (UserStatus)random.Next(1, 4),
                };

                var sofiaAdminPass = $"sofiaAdmin{i}";
                var sofiaAdminResult = await userManager.CreateAsync(sofiaAdmin, sofiaAdminPass);

                if (sofiaAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(sofiaAdmin, GlobalConstants.ArenaAdminRoleName);
                }
            }

            // Plovdiv admins (2)
            for (int i = 1; i <= 2; i++)
            {
                var plovdivAdmin = new ApplicationUser
                {
                    UserName = $"plovdivAdmin{i}",
                    Email = $"plovdivAdmin{i}@letssport.com",
                    FirstName = $"Plovdiv{i}",
                    LastName = $"Plovdiev{i}",
                    CountryId = bulgariaId,
                    CityId = await dbContext.Cities.Where(c => c.Name == "Plovdiv").Select(c => c.Id).FirstAsync(),
                    Age = 18 + i,
                    Gender = (Gender)1,
                    SportId = random.Next(1, 31),
                    Status = (UserStatus)random.Next(1, 4),
                };

                var plovdivAdminPass = $"plovdivAdmin{i}";
                var plovdivAdminResult = await userManager.CreateAsync(plovdivAdmin, plovdivAdminPass);

                if (plovdivAdminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(plovdivAdmin, GlobalConstants.ArenaAdminRoleName);
                }
            }

            // Varna admin
            var varnaAdmin = new ApplicationUser
            {
                UserName = $"varnaAdmin",
                Email = $"varnaAdmin@letssport.com",
                FirstName = $"Varna",
                LastName = $"Varneva",
                CountryId = bulgariaId,
                CityId = await dbContext.Cities.Where(c => c.Name == "Varna").Select(c => c.Id).FirstAsync(),
                Age = 22,
                Gender = (Gender)2,
                SportId = random.Next(1, 31),
                Status = (UserStatus)random.Next(1, 4),
            };

            var varnaAdminPass = $"varnaAdmin";
            var varnaAdminResult = await userManager.CreateAsync(varnaAdmin, varnaAdminPass);

            if (varnaAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(varnaAdmin, GlobalConstants.ArenaAdminRoleName);
            }

            // Burgas admin
            var burgasAdmin = new ApplicationUser
            {
                UserName = $"burgasAdmin",
                Email = $"burgasAdmin@letssport.com",
                FirstName = $"Burgas",
                LastName = $"Burgasov",
                CountryId = bulgariaId,
                CityId = await dbContext.Cities.Where(c => c.Name == "Burgas").Select(c => c.Id).FirstAsync(),
                Age = 24,
                Gender = (Gender)3,
                SportId = random.Next(1, 31),
                Status = (UserStatus)random.Next(1, 4),
            };

            var burgasAdminPass = $"burgasAdmin";
            var burgasAdminResult = await userManager.CreateAsync(burgasAdmin, burgasAdminPass);

            if (burgasAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(burgasAdmin, GlobalConstants.ArenaAdminRoleName);
            }

            // Ruse admin
            var ruseAdmin = new ApplicationUser
            {
                UserName = $"ruseAdmin",
                Email = $"ruseAdmin@letssport.com",
                FirstName = $"Ruse",
                LastName = $"Rusev",
                CountryId = bulgariaId,
                CityId = await dbContext.Cities.Where(c => c.Name == "Ruse").Select(c => c.Id).FirstAsync(),
                Age = 24,
                Gender = (Gender)1,
                SportId = random.Next(1, 31),
                Status = (UserStatus)random.Next(1, 4),
            };

            var ruseAdminPass = $"ruseAdmin";
            var ruseAdminResult = await userManager.CreateAsync(ruseAdmin, ruseAdminPass);

            if (ruseAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(ruseAdmin, GlobalConstants.ArenaAdminRoleName);
            }
        }
    }
}
