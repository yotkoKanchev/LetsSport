namespace LetsSport.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using Microsoft.EntityFrameworkCore;

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
            var rentalRequest = await this.rentalRequestsRepository
                .All()
                .FirstOrDefaultAsync(rr => rr.Id == arenaRentalRequestId);

            return await this.eventsService.GetEventByIdAsync<EventInfoViewModel>(rentalRequest.EventId);
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

        public async Task SetPassedStatusAsync(int arenaId)
        {
            var query = this.rentalRequestsRepository
                .All()
                .Where(rr => rr.ArenaId == arenaId)
                .Where(rr => rr.Event.Date < DateTime.UtcNow);

            foreach (var req in query)
            {
                req.Status = ArenaRentalRequestStatus.Passed;
            }

            await this.rentalRequestsRepository.SaveChangesAsync();
        }

        public async Task ChangeStatus(string id, ArenaRentalRequestStatus status)
        {
            var rentalRequest = await this.rentalRequestsRepository
                .All()
                .FirstOrDefaultAsync();

            rentalRequest.Status = status;
            this.rentalRequestsRepository.Update(rentalRequest);
            await this.rentalRequestsRepository.SaveChangesAsync();
        }
    }
}
