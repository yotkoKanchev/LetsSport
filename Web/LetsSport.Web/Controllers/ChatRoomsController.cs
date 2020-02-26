namespace LetsSport.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class ChatRoomsController : BaseController
    {
        private readonly IMessageService messageService;

        public ChatRoomsController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public IActionResult ChatRoom()
        {
            return this.View();
        }

        [HttpGet]                                                       //TODO where we getting chatRoomId from ?!?
        public async Task<IActionResult> SendMessage(MessageCreateInputModel inputModel, string chatRoomId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.messageService.Create(inputModel, userId, chatRoomId);

            return this.View();
        }
    }
}
