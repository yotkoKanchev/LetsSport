namespace LetsSport.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels;
    using LetsSport.Web.ViewModels.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class ChatRoomsController : BaseController
    {
        private readonly IMessagesService messagesService;
        private readonly IRepository<Event> eventsRepository;
        private readonly IEventsService eventsService;

        public ChatRoomsController(IMessagesService messageService, IRepository<Event> eventsRepository, IEventsService eventsService)
        {
            this.messagesService = messageService;
            this.eventsRepository = eventsRepository;
            this.eventsService = eventsService;
        }

        public IActionResult ChatRoom(int id)
        {
            var viewModel = this.eventsRepository
                .All()
                .Where(e => e.Id == id)
                .Select(e => new ChatRoomViewModel
                {
                    EventId = e.Id,
                    Id = e.ChatRoom.Id,
                    EventName = e.Name,
                    Messages = e.ChatRoom
                        .Messages
                        .OrderByDescending(m => m.CreatedOn)
                        .Select(m => new MessageDetailsViewModel
                        {
                            CreatedOn = m.CreatedOn.ToString("dd-MM-yyy hh:mm"),
                            Sender = m.Sender.UserName,
                            Text = m.Text,
                        }).ToList(),
                })
                .FirstOrDefault();

            return this.View(viewModel);
        }

        [HttpPost]  //TODO change viewModelHere with inputModel, i can not do that because i can use only one model in view!!!
        public async Task<IActionResult> ChatRoom(ChatRoomViewModel inputModel)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.messagesService.Create(inputModel.Text, userId, inputModel.Id);

            var id = this.eventsService.GetIdByChatRoomId(inputModel.Id);

            return this.Redirect($"/chatrooms/chatroom/{id}");
        }
    }
}
