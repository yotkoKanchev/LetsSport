namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;

    public class SportsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Sports.Any())
            {
                return;
            }

            var sports = new List<Sport>()
            {
                new Sport { Name = "Basketball", Image = "https://images.unsplash.com/photo-1516802273409-68526ee1bdd6?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Football", Image = "https://images.unsplash.com/photo-1486286701208-1d58e9338013?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Volleyball", Image = "https://images.unsplash.com/photo-1553005746-9245ba190489?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Aquatics", Image = "https://images.unsplash.com/photo-1519315901367-f34ff9154487?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Archery", Image = "https://images.unsplash.com/photo-1538432091670-e6b79bd9bffa?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Athletics", Image = "https://images.unsplash.com/photo-1526676037777-05a232554f77?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Badminton", Image = "https://images.unsplash.com/photo-1564226803380-91139fdcb4d0?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Baseball", Image = "https://images.unsplash.com/photo-1525571296628-8c2ee4e47321?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Boxing", Image = "https://images.unsplash.com/photo-1495555687398-3f50d6e79e1e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "CanoeKayak", Image = "https://images.unsplash.com/photo-1544407558-71e53c6a1136?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Cycling", Image = "https://images.unsplash.com/photo-1541625602330-2277a4c46182?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Equestrian", Image = "https://images.unsplash.com/photo-1526038039141-92d734991065?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Fencing", Image = "https://images.unsplash.com/photo-1505619656705-59ebc350b547?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "FieldHockey", Image = "https://images.unsplash.com/photo-1537753068441-ae5962264fe7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Golf", Image = "https://images.unsplash.com/photo-1535132011086-b8818f016104?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Gymnastics", Image = "https://images.unsplash.com/photo-1516208813382-cbaf53501037?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Handball", Image = "https://images.unsplash.com/photo-1553627220-92f0446b6a5f?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "IceHockey", Image = "https://images.unsplash.com/photo-1581275701366-d9bfe401faa4?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Judo", Image = "https://images.unsplash.com/photo-1542937307-6eeb0267cbab?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Karate", Image = "https://images.unsplash.com/photo-1529630218527-7df22fc2d4ee?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Rowing", Image = "https://images.unsplash.com/photo-1558253917-0edb67da60e5?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Rugby", Image = "https://images.unsplash.com/photo-1480099225005-2513c8947aec?ixlib=rb-1.2.1&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Sailing", Image = "https://images.unsplash.com/photo-1506527240747-720a3e17b910?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Shooting", Image = "https://images.unsplash.com/photo-1562461094-e060ef34728e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Softball", Image = "https://images.unsplash.com/photo-1549840962-3f0db68248e2?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Squash", Image = "https://images.unsplash.com/photo-1554290813-ec6a2a72e5c7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "TableTennis", Image = "https://images.unsplash.com/photo-1461748659110-16121c049d52?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Taekwondo", Image = "https://images.unsplash.com/photo-1514050566906-8d077bae7046?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Tennis", Image = "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Triathlon", Image = "https://images.unsplash.com/photo-1533547477463-bcffb9ef386d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Weightlifting", Image = "https://images.unsplash.com/photo-1521804906057-1df8fdb718b7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
                new Sport { Name = "Wrestling", Image = "https://images.unsplash.com/photo-1541337082051-5959dbb57d5d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80" },
            };

            await dbContext.Sports.AddRangeAsync(sports);
        }
    }
}
