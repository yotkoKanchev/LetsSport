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
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class ArenasService : IArenasService
    {
        private const string InvalidArenaIdErrorMessage = "Arena with ID: {0} does not exist.";
        private const string UnexistingArenaIdErrorMessage = "Arena with name: {0} in {1} city, {2} does not exist.";
        private readonly IEmailSender emailSender;
        private readonly IAddressesService addressesService;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Arena> arenasRepository;
        private readonly IConfiguration configuration;
        private readonly string editImageSizing = "w_480,h_288,c_scale,r_5,bo_1px_solid_silver/";
        private readonly string detailsImageSizing = "w_384,h_216,c_scale,r_10,bo_3px_solid_silver/";
        private readonly string mainImageSizing = "w_768,h_432,c_scale,r_10,bo_3px_solid_silver/";

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";

        public ArenasService(
            IEmailSender emailSender,
            IAddressesService addressesService,
            IImagesService imagesService,
            ISportsService sportsService,
            IRepository<Arena> arenasRepository,
            IConfiguration configuration)
        {
            this.emailSender = emailSender;
            this.addressesService = addressesService;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.arenasRepository = arenasRepository;
            this.configuration = configuration;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<int> CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username)
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

            var sportName = this.sportsService.GetSportNameById(inputModel.SportId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.ArenaCreated,
                        EmailHtmlMessages.GetArenaCreationHtml(
                            username,
                            inputModel.Name,
                            sportName));

            return arena.Id;
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

        public ArenaDetailsViewModel GetDetails(int id)
        {
            var viewModel = this.GetArenaByIdAsIQuerable(id).To<ArenaDetailsViewModel>().FirstOrDefault();
            viewModel.MainImageUrl = this.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = this.GetImageUrslById(id);

            return viewModel;
        }

        public MyArenaDetailsViewModel GetMyArenaDetails(int id)
        {
            var viewModel = this.GetArenaByIdAsIQuerable(id).To<MyArenaDetailsViewModel>().FirstOrDefault();
            viewModel.MainImageUrl = this.SetMainImage(viewModel.MainImageUrl);
            viewModel.Pictures = this.GetImageUrslById(id);

            return viewModel;
        }

        public IEnumerable<T> GetAll<T>((string City, string Country) location)
        {
            var query = this.arenasRepository
                .All()
                .Where(a => a.Status == ArenaStatus.Active)
                .Where(a => a.Address.City.Name == location.City)
                .Where(c => c.Address.City.Country.Name == location.Country)
                .OrderBy(a => a.Name);

            var arenas = query.To<T>();

            return arenas.ToList();
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

        public IEnumerable<SelectListItem> GetAllArenas((string City, string Country) location)
        {
            var arenas = this.GetAll<ArenaToSelectListItemViewModel>(location);

            return arenas.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id,
            });
        }

        public IEnumerable<ArenaIndexInfoViewModel> GetArenasByCityId(int cityId)
        {
            var query = this.arenasRepository
                .All()
                .Where(a => a.Address.CityId == cityId);

            return query.To<ArenaIndexInfoViewModel>().ToList();
        }

        public bool IsArenaExists(string userId) => this.GetArenaIdByAdminId(userId) > 0;

        private Arena GetArenaById(int arenaId) => this.arenasRepository
            .All()
            .FirstOrDefault(a => a.Id == arenaId);

        private IQueryable GetArenaByIdAsIQuerable(int arenaId)
        {
            var query = this.arenasRepository.All()
                .Where(a => a.Id == arenaId);

            if (query == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, arenaId));
            }

            return query;
        }

        private string SetMainImage(string imageUrl)
        {
            var resultUrl = "../../images/noArena.png";

            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imagePath = this.imagesService.ConstructUrlPrefix(this.mainImageSizing);
                resultUrl = imagePath + imageUrl;
            }

            return resultUrl;
        }
    }
}
