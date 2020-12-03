namespace LetsSport.Services.Data.RentalRequests
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.Arenas;
    using Microsoft.EntityFrameworkCore;

    using static LetsSport.Common.ErrorMessages;

    public class RentalRequestsService : IRentalRequestsService
    {
        private readonly IRepository<ArenaRentalRequest> rentalRequestsRepository;

        public RentalRequestsService(IRepository<ArenaRentalRequest> rentalRequestsRepository)
        {
            this.rentalRequestsRepository = rentalRequestsRepository;
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

        public async Task ChangeStatusAsync(string id, ArenaRentalRequestStatus status)
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
