namespace LetsSport.Web.Hubs
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessagesService messagesService;

        public ChatHub(IMessagesService messagesService)
        {
            this.messagesService = messagesService;
        }

        public async Task SendMessage(string content, string eventId, string userId)
        {
            var eventIdAsInt = int.Parse(eventId);
            string id = await this.messagesService.CreateAsync(content, userId, eventIdAsInt);

            var message = await this.messagesService.GetDetailsById(id);

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, eventId);
            await this.Clients.Group(eventId).SendAsync("NewMessage", message);

            //await this.Clients.All.SendAsync("NewMessage", message);
        }
    }
}
