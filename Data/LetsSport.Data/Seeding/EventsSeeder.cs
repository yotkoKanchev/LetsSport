namespace LetsSport.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public class EventsSeeder : ISeeder
    {
        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();

            // TODO add few events in Sofia, Plovdiv, Varna, Burgas, Pleven, Stara Zagora, Veliko Tarnovo
        }
    }
}
