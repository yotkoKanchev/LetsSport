namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ReportsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Reports.Any())
            {
                return;
            }

            var random = new Random();
            var reports = new List<Report>();

            var adminRoleId = await dbContext.Roles
                .Where(r => r.Name == GlobalConstants.AdministratorRoleName)
                .Select(r => r.Id)
                .FirstAsync();

            var usersCount = await dbContext.ApplicationUsers
                .Where(u => !u.Roles
                    .Any(r => r.RoleId == adminRoleId))
                .CountAsync();

            for (int i = 0; i < 20; i++)
            {
                var senderId = await dbContext.ApplicationUsers
                    .Where(u => u.Id != adminRoleId)
                    .Skip(random.Next(1, usersCount + 1))
                    .Select(u => u.Id)
                    .FirstAsync();

                var recipientId = await dbContext.ApplicationUsers
                    .Where(u => u.Id != adminRoleId && u.Id != senderId)
                    .Skip(random.Next(1, usersCount + 1))
                    .Select(u => u.Id)
                    .FirstAsync();

                var report = new Report
                {
                    SenderId = senderId,
                    ReportedUserId = recipientId,
                    Content = "This user broke the rules few time",
                    Abuse = (AbuseType)random.Next(1, 6),
                };

                reports.Add(report);
            }

            dbContext.AddRange(reports);
        }
    }
}
