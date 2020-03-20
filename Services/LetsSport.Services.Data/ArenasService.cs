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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class ArenasService : IArenasService
    {
        private const string InvalidArenaIdErrorMessage = "Arena with ID: {0} does not exist.";
        private const string UnexistingArenaIdErrorMessage = "Arena with name: {0} in {1} city, {2} does not exist.";

        private readonly IAddressesService addressesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Arena> arenasRepository;
        private readonly IConfiguration configuration;
        private readonly string editImageSizing = "w_480,h_288,c_scale,r_5,bo_1px_solid_silver/";
        private readonly string detailsImageSizing = "w_384,h_216,c_scale,r_10,bo_3px_solid_silver/";

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";

        public ArenasService(
            IAddressesService addressesService,
            IImagesService imagesService,
            ISportsService sportsService,
            IRepository<Arena> arenasRepository,
            IConfiguration configuration)
        {
            this.addressesService = addressesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.arenasRepository = arenasRepository;
            this.configuration = configuration;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<int> CreateAsync(ArenaCreateInputModel inputModel, string userId)
        {
            var arena = inputModel.To<ArenaCreateInputModel, Arena>();
            arena.ArenaAdminId = userId;
            arena.AddressId = await this.addressesService.CreateAsync(inputModel.City, inputModel.StreetAddress);

            if (inputModel.MainImageFile != null)
            {
                var avatar = await this.imagesService.CreateAsync(inputModel.MainImageFile);
                arena.MainImageId = avatar.Id;
            }

            if (inputModel.ImageFiles != null)
            {
                foreach (var img in inputModel.ImageFiles)
                {
                    var image = await this.imagesService.CreateAsync(img);
                    arena.Images.Add(image);
                }
            }

            await this.arenasRepository.AddAsync(arena);
            await this.arenasRepository.SaveChangesAsync();

            return arena.Id;
        }

        public T GetDetails<T>(int id)
        {
            var query = this.arenasRepository.All().Where(a => a.Id == id);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, id));
            }

            var viewModel = query.To<T>().FirstOrDefault();

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

        public async Task ChangeMainImageAsync(int arenaId, IFormFile newMainImageFile)
        {
            var arena = this.GetArenaById(arenaId);
            var mainImageId = arena.MainImageId;

            var newMainImage = await this.imagesService.CreateAsync(newMainImageFile);
            arena.MainImageId = newMainImage.Id;

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();

            await this.imagesService.DeleteImageAsync(mainImageId);
        }

        public async Task DeleteMainImage(int arenaId)
        {
            var arena = this.GetArenaById(arenaId);
            var mainImageId = arena.MainImageId;

            arena.MainImageId = null;
            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();

            await this.imagesService.DeleteImageAsync(mainImageId);
        }

        public ArenaImagesEditViewModel GetArenasImagesByArenaId(int id)
        {
            var query = this.arenasRepository
                .All()
                .Where(a => a.Id == id);

            var viewModel = query.To<ArenaImagesEditViewModel>().FirstOrDefault();

            foreach (var image in viewModel.Images)
            {
                image.Url = this.imagePathPrefix + this.editImageSizing + image.Url;
            }

            return viewModel;
        }

        public int GetArenaIdByAdminId(string arenaAdminId)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.ArenaAdminId == arenaAdminId)
                .Select(a => a.Id)
                .FirstOrDefault();
        }

        public async Task AddImages(IEnumerable<IFormFile> newImages, int arenaId)
        {
            var arena = this.arenasRepository
                .All()
                .Where(a => a.Id == arenaId)
                .FirstOrDefault();

            if (newImages != null)
            {
                foreach (var img in newImages)
                {
                    var image = await this.imagesService.CreateAsync(img);
                    arena.Images.Add(image);
                }
            }

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public IEnumerable<string> GetImageUrslById(int id)
        {
            var shortenedUrls = this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .Select(a => a.Images
                    .Select(i => i.Url)
                    .ToList())
                .FirstOrDefault();

            var urls = this.imagesService.ConstructUrls(this.detailsImageSizing, shortenedUrls);
            return urls;
        }

        private Arena GetArenaById(int arenaId)
        {
            return this.arenasRepository.All()
                .FirstOrDefault(a => a.Id == arenaId);
        }
    }
}
