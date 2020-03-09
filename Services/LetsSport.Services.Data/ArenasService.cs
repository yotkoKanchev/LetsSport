namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Arenas;

    public class ArenasService : IArenasService
    {
        private readonly IAddressesService addressesService;
        private readonly IImagesService imagesService;
        private readonly IRepository<Arena> arenasRepository;

        private readonly string mainImageSizing = "w_768,h_432,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string imageSizing = "w_384,h_216,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string noArenaImageId = "noArena";

        public ArenasService(
            IAddressesService addressesService,
            IImagesService imagesService,
            IRepository<Arena> arenasRepository)
        {
            this.addressesService = addressesService;
            this.imagesService = imagesService;
            this.arenasRepository = arenasRepository;
        }

        public async Task<int> CreateAsync(ArenaCreateInputModel inputModel)
        {
            var addressId = await this.addressesService.CreateAsync(inputModel.Country, inputModel.City, inputModel.Address);
            var sportType = (SportType)Enum.Parse(typeof(SportType), inputModel.Sport);

            var mainImageId = inputModel.ProfilePicture != null
                ? await this.imagesService.CreateAsync(inputModel.ProfilePicture)
                : this.noArenaImageId;

            if (mainImageId == null)
            {
                mainImageId = this.noArenaImageId;
            }

            var arena = new Arena
            {
                Name = inputModel.Name,
                Sport = sportType,
                PhoneNumber = inputModel.PhoneNumber,
                AddressId = addressId,
                Description = inputModel.Description,
                PricePerHour = inputModel.PricePerHour,
                WebUrl = inputModel.WebUrl,
                Email = inputModel.Email,
                MainImageId = mainImageId,
            };

            if (inputModel.Pictures != null)
            {
                var images = await this.imagesService.CreateCollectionOfPicturesAsync(inputModel.Pictures);
                arena.Images = images;
            }

            await this.arenasRepository.AddAsync(arena);
            await this.arenasRepository.SaveChangesAsync();

            return arena.Id;
        }

        public ArenaDetailsViewModel GetDetails(int id)
        {
            var imagePathPrefix = this.imagesService.ConstructUrlPrefix(this.mainImageSizing);

            var inputModel = this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .Select(a => new ArenaDetailsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Sport = a.Sport.ToString(),
                    Address = a.Address.StreetAddress + ", " + a.Address.City.Name + ", " + a.Address.City.Country.Name,
                    PhoneNumber = a.PhoneNumber,
                    WebUrl = a.WebUrl,
                    Email = a.Email,
                    PricePerHour = a.PricePerHour.ToString("F2"),
                    ArenaAdmin = a.ArenaAdmin.UserName,
                    Rating = a.Events.Count.ToString("F2"),
                    MainImage = imagePathPrefix + a.MainImage.Url,
                })
                .FirstOrDefault();

            inputModel.Pictures = this.GetImageUrslById(id);
            return inputModel;
        }

        public ArenaEditViewModel GetArenaForEdit(int id)
        {
            var viewModel = this.arenasRepository
                .AllAsNoTracking()
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
                    Email = a.Email,
                    Description = a.Description,
                    Country = a.Address.City.Country.Name,
                    City = a.Address.City.Name,
                    StreetAddress = a.Address.StreetAddress,
                })
                .FirstOrDefault();

            return viewModel;
        }

        public async Task UpdateArenaAsync(ArenaEditViewModel viewModel)
        {
            var arena = this.arenasRepository
                .All()
                .FirstOrDefault(a => a.Id == viewModel.Id);

            // TODO check if all are null
            arena.Name = viewModel.Name;
            arena.PhoneNumber = viewModel.PhoneNumber;
            arena.PricePerHour = viewModel.PricePerHour;
            arena.Description = viewModel.Description;
            arena.Sport = viewModel.SportType != null
                ? (SportType)Enum.Parse(typeof(SportType), viewModel.SportType)
                : arena.Sport;
            arena.WebUrl = viewModel.WebUrl;
            arena.Email = viewModel.Email;

            await this.addressesService.UpdateAddressAsync(arena.AddressId, viewModel.StreetAddress);

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public int GetArenaId(string name, string city, string country)
        {
            return this.arenasRepository
                .AllAsNoTracking()
                .Where(a => a.Name == name)
                .Where(a => a.Address.City.Name == city &&
                            a.Address.City.Country.Name == country)
                .Select(a => a.Id)
                .FirstOrDefault();
        }

        public IEnumerable<string> GetArenas(string city, string country)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.Address.City.Name == city)
                .Where(c => c.Address.City.Country.Name == country)
                .Select(c => c.Name)
                .ToList();

            return arenas;
        }

        private IEnumerable<string> GetImageUrslById(int id)
        {
            var shortenedUrls = this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .Select(a => a.Images
                    .Select(i => i.Url)
                    .ToList())
                .FirstOrDefault();

            var urls = this.imagesService.ConstructUrls(this.imageSizing, shortenedUrls);
            return urls;
        }
    }
}
