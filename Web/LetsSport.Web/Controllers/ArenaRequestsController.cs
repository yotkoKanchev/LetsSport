namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Data;
    using LetsSport.Web.Filters;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.ConfirmationMessages;
    using static LetsSport.Common.GlobalConstants;

    [Authorize]
    [Authorize(Roles = ArenaAdminRoleName)]
    [ServiceFilter(typeof(SetLocationResourceFilter))]

    public class ArenaRequestsController : BaseController
    {
        private readonly IRentalRequestsService rentalRequestsService;
        private readonly IEventsService eventsService;

        public ArenaRequestsController(IRentalRequestsService rentalRequestsService, IEventsService eventsService)
        {
            this.rentalRequestsService = rentalRequestsService;
            this.eventsService = eventsService;
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            var eventDetails = await this.eventsService.GetEventByRequestIdAsync(id);

            if (eventDetails == null)
            {
                return this.RedirectToAction("NotFoundError", "Error");
            }

            var viewModel = new RequestViewModel
            {
                Id = id,
                EventId = eventDetails.Id,
                EventInfo = eventDetails,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id, int eventId)
        {
            await this.rentalRequestsService.ChangeStatusAsync(id, ArenaRentalRequestStatus.Approved);
            await this.eventsService.ChangeStatus(eventId, ArenaRequestStatus.Approved);
            this.TempData[TempDataMessage] = string.Format(ApprovedEvent, eventId);

            return this.RedirectToAction("Events", "Arenas");
        }

        [HttpPost]
        public async Task<IActionResult> Deny(string id, int eventId)
        {
            await this.rentalRequestsService.ChangeStatusAsync(id, ArenaRentalRequestStatus.Denied);
            await this.eventsService.ChangeStatus(eventId, ArenaRequestStatus.Denied);
            this.TempData[TempDataMessage] = string.Format(DeniedEvent, eventId);

            return this.RedirectToAction("Events", "Arenas");
        }
    }
}
