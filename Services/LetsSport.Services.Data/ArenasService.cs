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
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ArenasService : IArenasService
    {
        private readonly IAddressesService addressesService;
        private readonly IImagesService imagesService;
        private readonly IRepository<Arena> arenasRepository;

        private readonly string mainImageSizing = "w_768,h_432,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string imageSizing = "w_384,h_216,c_scale,r_10,bo_2px_solid_blue/";
        private readonly string noArenaUrl = "v1583681459/noImages/noArena_jpgkez.png";
        //private readonly string noArenaImageId = "noArena";

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
            var arena = new Arena
            {
                Name = inputModel.Name,
                SportId = inputModel.Sport,
                PhoneNumber = inputModel.PhoneNumber,
                AddressId = await this.addressesService.CreateAsync(inputModel.City, inputModel.Address),
                Description = inputModel.Description,
                PricePerHour = inputModel.PricePerHour,
                WebUrl = inputModel.WebUrl,
                Email = inputModel.Email,
                MainImageId = await this.imagesService.CreateAsync(inputModel.ProfilePicture, this.noArenaUrl),
            };

            if (inputModel.Pictures != null)
            {
                var images = await this.imagesService.CreateCollectionOfPicturesAsync(inputModel.Pictures, this.noArenaUrl);
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
            arena.SportId = 1; /*TODO GET SPORT ID*/
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

        public IEnumerable<SelectListItem> GetArenas(string city, string country)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.Address.City.Name == city)
                .Where(c => c.Address.City.Country.Name == country)
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
