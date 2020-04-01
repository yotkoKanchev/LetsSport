namespace LetsSport.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public class ArenasSeeder : ISeeder
    {
        public Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();

            // TODO add 10 arenas in Sofia, 5 in Plovdiv, 3 in Varna, Burgas, 1 in Ruse, Pleven, Stara Zagora, Veliko Tarnovo
        }
    }
}
