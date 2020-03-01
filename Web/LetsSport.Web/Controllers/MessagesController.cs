namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class MessagesController : BaseController
    {
        private readonly IMessagesService messagesService;
        private readonly IEventsService eventsService;

        public MessagesController(IMessagesService messagesService, IEventsService eventsService)
        {
            this.messagesService = messagesService;
            this.eventsService = eventsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChatRoomViewModel inputModel)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.messagesService.CreateMessageAsync(inputModel.Text, userId, inputModel.Id);

            var eventId = this.eventsService.GetIdByChatRoomId(inputModel.Id);
            return this.RedirectToAction($"/ChatRooms/ChatRoom{eventId}");
        }
    }
}
