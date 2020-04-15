namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.GlobalConstants;
    using static LetsSport.Web.Common.ConfirmationMessages;

    [Authorize]
    [Authorize(Roles = ArenaAdminRoleName)]
    public class ArenaRequestsController : BaseController
    {
        private readonly IRentalRequestsService rentalRequestsService;
        private readonly IEventsService eventsService;

        public ArenaRequestsController(
            ILocationLocator locator,
            IRentalRequestsService rentalRequestsService,
            IEventsService eventsService)
            : base(locator)
        {
            this.rentalRequestsService = rentalRequestsService;
            this.eventsService = eventsService;
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            var request = await this.rentalRequestsService.GetDetails(id);

            var viewModel = new RequestViewModel
            {
                Id = id,
                EventId = request.Id,
                EventInfo = request,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id, int eventId)
        {
            await this.rentalRequestsService.ChangeStatus(id, ArenaRentalRequestStatus.Approved);
            await this.eventsService.ChangeStatus(eventId, ArenaRequestStatus.Approved);
            this.TempData[TempDataMessage] = string.Format(ApprovedEvent, eventId);

            return this.RedirectToAction("Events", "Arenas");
        }

        [HttpPost]
        public async Task<IActionResult> Deny(string id, int eventId)
        {
            await this.rentalRequestsService.ChangeStatus(id, ArenaRentalRequestStatus.Denied);
            await this.eventsService.ChangeStatus(eventId, ArenaRequestStatus.Denied);
            this.TempData[TempDataMessage] = string.Format(DeniedEvent, eventId);

            return this.RedirectToAction("Events", "Arenas");
        }
    }
}
