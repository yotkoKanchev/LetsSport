namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenasService : IArenasService
    {
        private const string InvalidArenaIdErrorMessage = "Arena with ID: {0} does not exist.";
        private const string UnexistingArenaIdErrorMessage = "Arena with name: {0} in {1} city, {2} does not exist.";

        private readonly IAddressesService addressesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Arena> arenasRepository;

        private readonly string mainImageSizing = "w_768,h_432,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string imageSizing = "w_384,h_216,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string noArenaUrl = "v1583681459/noImages/noArena_jpgkez.png";

        // private readonly string noArenaImageId = "noArena";
        public ArenasService(
            IAddressesService addressesService,
            IImagesService imagesService,
            ISportsService sportsService,
            IRepository<Arena> arenasRepository)
        {
            this.addressesService = addressesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.arenasRepository = arenasRepository;
        }

        public async Task<int> CreateAsync(ArenaCreateInputModel inputModel)
        {
            var arena = inputModel.To<ArenaCreateInputModel, Arena>();

            arena.AddressId = await this.addressesService.CreateAsync(inputModel.City, inputModel.StreetAddress);
            arena.MainImageId = await this.imagesService.CreateAsync(inputModel.ProfilePicture, this.noArenaUrl);

            if (inputModel.Pictures != null)
            {
                var images = await this.imagesService.CreateImagesCollectionAsync(inputModel.Pictures, this.noArenaUrl);
                arena.Images = images;
            }

            await this.arenasRepository.AddAsync(arena);
            await this.arenasRepository.SaveChangesAsync();

            return arena.Id;
        }

        public ArenaDetailsViewModel GetDetails(int id)
        {
            var query = this.arenasRepository.All().Where(a => a.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, id));
            }

            var imagePath = this.imagesService.ConstructUrlPrefix(this.mainImageSizing);
            var viewModel = query.To<ArenaDetailsViewModel>().FirstOrDefault();
            viewModel.MainImageUrl = imagePath + viewModel.MainImageUrl;
            viewModel.Pictures = this.GetImageUrslById(id);

            return viewModel;
        }

        public ArenaEditViewModel GetArenaForEdit(int id)
        {
            var query = this.arenasRepository
                .AllAsNoTracking()
                .Where(a => a.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, id));
            }

            var viewModel = query.To<ArenaEditViewModel>().FirstOrDefault();
            viewModel.Sports = this.sportsService.GetAll();
            return viewModel;
        }

        public async Task UpdateArenaAsync(ArenaEditViewModel viewModel)
        {
            var arena = this.arenasRepository
                .All()
                .FirstOrDefault(a => a.Id == viewModel.Id);

            if (arena == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, viewModel.Id));
            }

            // TODO check if all are null
            arena.Name = viewModel.Name;
            arena.PhoneNumber = viewModel.PhoneNumber;
            arena.PricePerHour = viewModel.PricePerHour;
            arena.Description = viewModel.Description;
            arena.SportId = viewModel.SportId;
            arena.WebUrl = viewModel.WebUrl;
            arena.Email = viewModel.Email;

            await this.addressesService.UpdateAddressAsync(arena.AddressId, viewModel.AddressStreetAddress);

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public int GetArenaId(string name, string city, string country)
        {
            var arenaId = this.arenasRepository
                .AllAsNoTracking()
                .Where(a => a.Name == name)
                .Where(a => a.Address.City.Name == city &&
                            a.Address.City.Country.Name == country)
                .Select(a => a.Id)
                .FirstOrDefault();

            if (arenaId == 0)
            {
                throw new ArgumentNullException(string.Format(UnexistingArenaIdErrorMessage, name, city, country));
            }

            return arenaId;
        }

        public IEnumerable<SelectListItem> GetArenas((string City, string Country) location)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.Address.City.Name == location.City)
                .Where(c => c.Address.City.Country.Name == location.Country)
                .OrderBy(a => a.Name);

            var resultList = new List<SelectListItem>();

            foreach (var arena in arenas)
            {
                resultList.Add(new SelectListItem { Value = arena.Id.ToString(), Text = arena.Name });
            }

            return resultList;
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
