namespace LetsSport.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ConfirmationMessages;

    public class MessagesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Messages.Any())
            {
                return;
            }

            var games = await dbContext.Events.ToListAsync();
            var messages = new List<Message>();

            foreach (var game in games)
            {
                var message = new Message
                {
                    EventId = game.Id,
                    SenderId = game.AdminId,
                    Content = EventCreationMessage,
                };

                messages.Add(message);
            }

            dbContext.AddRange(messages);
        }
    }
}
