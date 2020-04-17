namespace LetsSport.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;

    public class RentalRequestsService : IRentalRequestsService
    {
        private readonly IRepository<ArenaRentalRequest> rentalRequestsRepository;
        private readonly IEventsService eventsService;

        public RentalRequestsService(IRepository<ArenaRentalRequest> rentalRequestsRepository, IEventsService eventsService)
        {
            this.rentalRequestsRepository = rentalRequestsRepository;
            this.eventsService = eventsService;
        }

        public async Task<EventInfoViewModel> GetDetails(string arenaRentalRequestId)
        {
            var eventId = await this.rentalRequestsRepository
                .All()
                .Where(rr => rr.Id == arenaRentalRequestId)
                .Select(rr => rr.EventId)
                .FirstOrDefaultAsync();

            if (eventId == 0)
            {
                throw new ArgumentException(string.Format(RentalRequestInvalidIdErrorMessage, arenaRentalRequestId));
            }

            return await this.eventsService.GetEventByIdAsync<EventInfoViewModel>(eventId);
        }

        public async Task CreateAsync(int eventId, int arenaId)
        {
            var rentalRequest = new ArenaRentalRequest
            {
                ArenaId = arenaId,
                EventId = eventId,
                Status = ArenaRentalRequestStatus.NotApproved,
            };

            await this.rentalRequestsRepository.AddAsync(rentalRequest);
            await this.rentalRequestsRepository.SaveChangesAsync();
        }

        public async Task ChangeStatus(string id, ArenaRentalRequestStatus status)
        {
            var rentalRequest = await this.rentalRequestsRepository
                .All()
                .FirstOrDefaultAsync(rr => rr.Id == id);

            if (rentalRequest == null)
            {
                throw new ArgumentException(string.Format(RentalRequestInvalidIdErrorMessage, id));
            }

            rentalRequest.Status = status;
            this.rentalRequestsRepository.Update(rentalRequest);
            await this.rentalRequestsRepository.SaveChangesAsync();
        }
    }
}
