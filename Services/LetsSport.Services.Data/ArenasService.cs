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
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class ArenasService : IArenasService
    {
        private const string InvalidArenaIdErrorMessage = "Arena with ID: {0} does not exist.";
        private readonly ICitiesService citiesService;
        private readonly IEmailSender emailSender;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Arena> arenasRepository;
        private readonly IConfiguration configuration;
        private readonly ICountriesService countriesService;
        private readonly string editImageSizing = "w_480,h_288,c_scale,r_5,bo_1px_solid_silver/";
        private readonly string detailsImageSizing = "w_384,h_216,c_scale,r_10,bo_3px_solid_silver/";
        private readonly string mainImageSizing = "w_768,h_432,c_scale,r_10,bo_3px_solid_silver/";

        private readonly string imagePathPrefix;
        private readonly string cloudinaryPrefix = "https://res.cloudinary.com/{0}/image/upload/";

        public ArenasService(
            ICitiesService citiesService,
            IEmailSender emailSender,
            IImagesService imagesService,
            ISportsService sportsService,
            IRepository<Arena> arenasRepository,
            IConfiguration configuration,
            ICountriesService countriesService)
        {
            this.citiesService = citiesService;
            this.emailSender = emailSender;
            this.imagesService = imagesService;
            this.sportsService = sportsService;
            this.arenasRepository = arenasRepository;
            this.configuration = configuration;
            this.countriesService = countriesService;
            this.configuration = configuration;
            this.imagePathPrefix = string.Format(this.cloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<IEnumerable<T>> GetAllInCityAsync<T>(int cityId)
        {
            return await this.GetAllActiveInCityAsIQueryable(cityId)
                 .To<T>()
                 .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllActiveInCitySelectListAsync(int cityId)
        {
            return await this.GetAllActiveInCityAsIQueryable(cityId)
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                })
                .ToListAsync();
        }

        public T GetById<T>(int id)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .To<T>()
                .FirstOrDefault();
        }

        // TODO make it async
        public int GetIdByAdminId(string arenaAdminId)
        {
            var arenaId = this.arenasRepository
                .All()
                .Where(a => a.ArenaAdminId == arenaAdminId)
                .Select(a => a.Id)
                .FirstOrDefault();

            if (arenaId == 0)
            {
                throw new ArgumentException($"User with ID: {arenaAdminId} does not have arena!");
            }

            return arenaId;
        }

        public async Task CreateAsync(ArenaCreateInputModel inputModel, string userId, string userEmail, string username)
        {
            var arena = inputModel.To<Arena>();
            arena.ArenaAdminId = userId;

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
        }

        public T GetDetails<T>(int id)
        {
            var viewModel = this.GetArenaByIdAsIQueryable(id).To<T>().FirstOrDefault();

            return viewModel;
        }

        public ArenaEditViewModel GetDetailsForEdit(int id)
        {
            var viewModel = this.GetArenaByIdAsIQueryable(id).To<ArenaEditViewModel>().FirstOrDefault();

            viewModel.Sports = this.sportsService.GetAllAsSelectList();
            return viewModel;
        }

        public async Task UpdateAsync(ArenaEditViewModel viewModel)
        {
            var arena = this.GetArenaById(viewModel.Id);

            arena.Name = viewModel.Name;
            arena.PhoneNumber = viewModel.PhoneNumber;
            arena.PricePerHour = viewModel.PricePerHour;
            arena.Description = viewModel.Description;
            arena.SportId = viewModel.SportId;
            arena.WebUrl = viewModel.WebUrl;
            arena.Email = viewModel.Email;
            arena.Status = viewModel.Status;
            arena.Address = viewModel.Address;

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public async Task<ArenaIndexListViewModel> FilterAsync(int countryId, int sport, int city)
        {
            var query = this.GetAllActiveInCountryAsIQueryable<ArenaCardPartialViewModel>(countryId);

            if (sport != 0)
            {
                query = query.Where(a => a.SportId == sport);
            }

            if (city != 0)
            {
                query = query.Where(a => a.CityId == city);
            }

            IEnumerable<SelectListItem> sports;

            if (city == 0)
            {
                sports = await this.sportsService.GetAllSportsInCountryByIdAsync(countryId);
            }
            else
            {
                sports = query
                    .Select(a => new SelectListItem
                    {
                        Text = a.SportName,
                        Value = a.SportId.ToString(),
                    })
                    .Distinct();
            }

            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = query.ToList(),
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithArenasInCountryAsync(countryId),
                    Sports = sports,
                },
            };

            return viewModel;
        }

        // TODO refactor all images methods
        // images methods
        public string SetMainImage(string imageUrl)
        {
            var resultUrl = "../../images/noArena.png";

            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imagePath = this.imagesService.ConstructUrlPrefix(this.mainImageSizing);
                resultUrl = imagePath + imageUrl;
            }

            return resultUrl;
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

        public async Task DeleteMainImageAsync(int arenaId)
        {
            var arena = this.GetArenaById(arenaId);
            var mainImageId = arena.MainImageId;
            arena.MainImageId = null;
            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
            await this.imagesService.DeleteImageAsync(mainImageId);
        }

        public ArenaImagesEditViewModel GetImagesById(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);
            var viewModel = query.To<ArenaImagesEditViewModel>().FirstOrDefault();

            foreach (var image in viewModel.Images)
            {
                image.Url = this.imagePathPrefix + this.editImageSizing + image.Url;
            }

            return viewModel;
        }

        public async Task AddImagesAsync(IEnumerable<IFormFile> newImages, int arenaId)
        {
            var arena = this.GetArenaById(arenaId);

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

        public IEnumerable<string> GetImagesUrslById(int id)
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

        // Admin
        public async Task<IEnumerable<SelectListItem>> GetAllInCitySelectListAsync(int? cityId)
        {
            return await this.arenasRepository
                .All()
                .Where(a => a.CityId == cityId)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId)
        {
            return await this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .OrderBy(a => a.City.Name)
                .ThenBy(a => a.Name)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IndexViewModel> AdminFilterAsync(int countryId, int? cityId, int? sportId, int? isDeleted)
        {
            var query = this.arenasRepository
                 .All()
                 .Where(a => a.CountryId == countryId);

            if (cityId != null)
            {
                query = query
                    .Where(a => a.CityId == cityId);
            }

            if (sportId != null)
            {
                query = query
                    .Where(a => a.SportId == sportId);
            }

            if (isDeleted != 0)
            {
                if (isDeleted == 1)
                {
                    query = query
                        .Where(c => c.IsDeleted == false);
                }
                else if (isDeleted == 2)
                {
                    query = query
                        .Where(c => c.IsDeleted == true);
                }
            }

            var arenas = query
                 .OrderBy(c => c.City.Name)
                 .ThenBy(c => c.Name)
                 .To<InfoViewModel>()
                 .ToList();

            var arenaName = this.countriesService.GetNameById(countryId);
            var countryName = this.countriesService.GetNameById(countryId);
            var location = cityId != null
                ? this.citiesService.GetNameById(cityId.Value) + ", " + countryName
                : countryName;

            var viewModel = new IndexViewModel
            {
                CountryId = countryId,
                Arenas = arenas,
                Location = location,
                Filter = new FilterBarViewModel
                {
                    City = cityId,
                    Sport = sportId,
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllSportsInCountryByIdAsync(countryId),
                },
            };

            return viewModel;
        }

        public async Task AdminUpdateAsync(EditViewModel inputModel)
        {
            var arena = this.GetArenaById(inputModel.Id);

            arena.Name = inputModel.Name;
            arena.SportId = inputModel.SportId;
            arena.PricePerHour = inputModel.PricePerHour;
            arena.PhoneNumber = inputModel.PhoneNumber;
            arena.WebUrl = inputModel.WebUrl;
            arena.Email = inputModel.Email;
            arena.Status = inputModel.Status;
            arena.Address = inputModel.Address;
            arena.Description = inputModel.Description;

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var arena = this.arenasRepository
                .All()
                .FirstOrDefault(a => a.Id == id);
            this.arenasRepository.Delete(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        // Helpers
        private Arena GetArenaById(int arenaId)
        {
            var arena = this.arenasRepository
            .All()
            .FirstOrDefault(a => a.Id == arenaId);

            if (arena == null)
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, arenaId));
            }

            return arena;
        }

        private IQueryable GetArenaByIdAsIQueryable(int arenaId)
        {
            var query = this.arenasRepository.All()
                .Where(a => a.Id == arenaId);

            if (!query.Any())
            {
                throw new ArgumentNullException(string.Format(InvalidArenaIdErrorMessage, arenaId));
            }

            return query;
        }

        private IEnumerable<T> GetAllActiveInCountryAsIQueryable<T>(int countryId)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .Where(a => a.Status == ArenaStatus.Active)
                .OrderBy(a => a.City.Name)
                .ThenBy(a => a.Name)
                .To<T>();
        }

        private IQueryable<Arena> GetAllActiveInCityAsIQueryable(int cityId)
        {
            return this.arenasRepository
                 .All()
                 .Where(a => a.CityId == cityId)
                 .Where(a => a.Status == ArenaStatus.Active)
                 .OrderBy(a => a.Name);
        }
    }
}
