namespace LetsSport.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Data;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Arenas;

    public class ArenasService : IArenasService
    {
        private readonly IAddressesService addressesService;
        private readonly ApplicationDbContext db;

        public ArenasService(IAddressesService addressesService, ApplicationDbContext db)
        {
            this.addressesService = addressesService;
            this.db = db;
        }

        public async Task Create(ArenaCreateInputModel inputModel)
        {
            var addressId = await this.addressesService.Create(inputModel.Country, inputModel.Country, inputModel.Address);
            var sportType = (SportType)Enum.Parse(typeof(SportType), inputModel.Sport);

            var arena = new Arena
            {
                Name = inputModel.Name,
                Sport = sportType,
                CreatedOn = DateTime.UtcNow,
                PhoneNumber = inputModel.PhoneNumber,
                AddressId = addressId,
                Description = inputModel.Description,
                PricePerHour = inputModel.PricePerHour,
                WebUrl = inputModel.WebUrl,
            };

            await this.db.Arenas.AddAsync(arena);
            await this.db.SaveChangesAsync();
        }
    }
}
