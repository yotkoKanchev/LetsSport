namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.EventModels;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;

        public UsersService(IDeletableEntityRepository<ApplicationUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public IList<Event> GetUserEvents(string userId)
        {
            var events = this.usersRepository
                .All()
                .Where(u => u.Id == userId)
                .Select(u => u.Events
                    .Select(ue => ue.Event)
                    .ToList())
                .FirstOrDefault();

            return events;
        }
    }
}
