namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Arenas;

    public class ArenasService : IArenasService
    {
        private readonly IAddressesService addressesService;
        private readonly ApplicationDbContext db;
        private readonly string currentCityName;
        private readonly string currentCountryName;

        public ArenasService(IAddressesService addressesService, ApplicationDbContext db, ILocationLocator locator)
        {
            this.addressesService = addressesService;
            this.db = db;

            var currentLocation = locator.GetLocationInfo();
            this.currentCityName = currentLocation.City;
            this.currentCountryName = currentLocation.Country;
        }

        public async Task CreateAsync(ArenaCreateInputModel inputModel)
        {
            var addressId = await this.addressesService.Create(inputModel.Country, inputModel.City, inputModel.Address);
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

        public ArenaDetailsViewModel GetArena(int id)
        {
            var inputModel = this.db.Arenas
                .Where(a => a.Id == id)
                .Select(a => new ArenaDetailsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Sport = a.Sport.ToString(),
                    Address = a.Address.StreetAddress + ", " + a.Address.City.Name + ", " + a.Address.City.Country.Name,
                    PhoneNumber = a.PhoneNumber,
                    WebUrl = a.WebUrl,
                    PricePerHour = a.PricePerHour,
                    ArenaAdmin = a.ArenaAdmin.UserName,
                    Description = a.Description,
                })
                .FirstOrDefault();

            return inputModel;
        }

        public ArenaEditViewModel GetArenaForEdit(int id)
        {
            var viewModel = this.db.Arenas
                .Where(a => a.Id == id)
                .Select(a => new ArenaEditViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    SportType = a.Sport.ToString(),
                    Address = a.Address.StreetAddress + ", " + a.Address.City.Name + ", " + a.Address.City.Country.Name,
                    PhoneNumber = a.PhoneNumber,
                    PricePerHour = a.PricePerHour,
                    WebUrl = a.WebUrl,
                    Description = a.Description,
                })
                .FirstOrDefault();

            return viewModel;
        }

        public int GetArenaId(string name)
        {
            return this.db.Arenas
                .Where(a => a.Name == name)
                .Where(a => a.Address.City.Name == this.currentCityName && a.Address.City.Country.Name == this.currentCountryName)
                .Select(a => a.Id)
                .FirstOrDefault();
        }

        public IEnumerable<string> GetArenas()
        {
            var arenas = this.db.Arenas
                .Where(a => a.Address.City.Name == this.currentCityName)
                .Where(c => c.Address.City.Country.Name == this.currentCountryName)
                .Select(c => c.Name)
                .ToList();

            return arenas;
        }
    }
}
