namespace LetsSport.Web.ViewComponents
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data.Messages;
    using LetsSport.Services.Data.Sports;
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

        public async Task<IViewComponentResult> InvokeAsync(string userId, int eventId, string sportName)
        {
            var viewModel = new ChatRoomViewModel
            {
                EventId = eventId,
                Sport = sportName,
                SportImage = await this.sportsService.GetImageByNameAsync(sportName),
                ChatRoomMessages = await this.messagesService.GetAllByEventIdAsync(eventId),
                UserId = userId,
            };

            return this.View(viewModel);
        }
    }
}
