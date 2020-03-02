namespace LetsSport.Services.Data
{
    using System.Collections.Generic;

    using LetsSport.Data.Models.EventModels;

    public interface IUsersService
    {
        IList<Event> GetUserEvents(string userId);
    }
}
