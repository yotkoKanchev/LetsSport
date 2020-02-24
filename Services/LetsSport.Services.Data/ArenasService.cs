namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Hosting;

    public class ArenasService : IArenasService
    {
        private readonly IAddressesService addressesService;
        private readonly ApplicationDbContext db;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly string currentCityName;
        private readonly string currentCountryName;

        public ArenasService(
            IAddressesService addressesService,
            ApplicationDbContext db,
            ILocationLocator locator,
            IHostingEnvironment hostingEnvironment)
        {
            this.addressesService = addressesService;
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            var currentLocation = locator.GetLocationInfo();
            this.currentCityName = currentLocation.City;
            this.currentCountryName = currentLocation.Country;
        }

        public async Task<int> CreateAsync(ArenaCreateInputModel inputModel)
        {
            var addressId = await this.addressesService.Create(inputModel.Country, inputModel.City, inputModel.Address);
            var sportType = (SportType)Enum.Parse(typeof(SportType), inputModel.Sport);

            string uniqueFileName = null;

            if (inputModel.Photo != null)
            {
                string uploadsFolder = Path.Combine(this.hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + inputModel.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                inputModel.Photo.CopyTo(fileStream);
            }

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
                PhotoPath = uniqueFileName,
            };

            await this.db.Arenas.AddAsync(arena);
            await this.db.SaveChangesAsync();

            return arena.Id;
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
                    PricePerHour = a.PricePerHour.ToString("F2"),
                    ArenaAdmin = a.ArenaAdmin.UserName,
                    Rating = a.Events.Count.ToString("F2"),
                    PhotoPath = a.PhotoPath,
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
                    Country = a.Address.City.Country.Name,
                    City = a.Address.City.Name,
                    StreetAddress = a.Address.StreetAddress,
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

        public void UpdateArena(ArenaEditViewModel viewModel)
        {
            var arena = this.db.Arenas.Find(viewModel.Id);
            arena.Name = viewModel.Name;
            arena.PhoneNumber = viewModel.PhoneNumber;
            arena.PricePerHour = viewModel.PricePerHour;
            arena.Description = viewModel.Description;
            arena.Sport = viewModel.SportType != null
                ? (SportType)Enum.Parse(typeof(SportType), viewModel.SportType)
                : arena.Sport;
            arena.WebUrl = viewModel.WebUrl;

            this.addressesService.UpdateAddress(arena.AddressId, viewModel.StreetAddress);

            this.db.Arenas.Update(arena);
            this.db.SaveChanges();
        }
    }
}
