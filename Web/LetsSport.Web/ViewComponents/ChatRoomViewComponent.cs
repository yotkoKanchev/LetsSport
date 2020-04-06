namespace LetsSport.Web.ViewComponents
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Mvc;

    public class ChatRoomViewComponent : ViewComponent
    {
        private readonly ISportsService sportsService;
        private readonly IMessagesService messagesService;

        public ChatRoomViewComponent(ISportsService sportsService, IMessagesService messagesService)
        {
            this.sportsService = sportsService;
            this.messagesService = messagesService;
        }

        public IViewComponentResult Invoke(string userId, int eventId, string sportName)
        {
            var viewModel = new ChatRoomViewModel
            {
                EventId = eventId,
                Sport = sportName,
                SportImage = this.sportsService.GetImageByName(sportName),
                ChatRoomMessages = this.messagesService.GetAllByEventId(eventId),
                UserId = userId,
            };

            return this.View(viewModel);
        }
    }
}
