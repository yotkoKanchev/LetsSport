namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class CitiesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Cities.Any())
            {
                return;
            }

            var cities = new List<string>
            {
                "Sofia",
                "Plovdiv",
                "Varna",
                "Burgas",
                "Ruse",
                "Stara Zagora",
                "Pleven",
                "Blagoevgrad",
                "Veliko Tarnovo",
                "Pazardzhik",
            };

            var bulgariaId = await dbContext.Countries.Where(c => c.Name == "Bulgaria").Select(c => c.Id).FirstAsync();
            var cityModels = cities.Select(c => new City { CountryId = bulgariaId, Name = c });

            await dbContext.Cities.AddRangeAsync(cityModels);
        }
    }
}
