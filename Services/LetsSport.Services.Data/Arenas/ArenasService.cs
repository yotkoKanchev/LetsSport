namespace LetsSport.Services.Data.Arenas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Data.Cities;
    using LetsSport.Services.Data.Countries;
    using LetsSport.Services.Data.Images;
    using LetsSport.Services.Data.Sports;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.ViewModels.Admin;
    using LetsSport.Web.ViewModels.Admin.Arenas;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using static LetsSport.Common.ErrorMessages;
    using static LetsSport.Common.GlobalConstants;

    public class ArenasService : IArenasService
    {
        private readonly ICitiesService citiesService;
        private readonly IEmailSender emailSender;
        private readonly IImagesService imagesService;
        private readonly ISportsService sportsService;
        private readonly IRepository<Arena> arenasRepository;
        private readonly IConfiguration configuration;
        private readonly ICountriesService countriesService;
        private readonly string imagePathPrefix;

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
            this.imagePathPrefix = string.Format(CloudinaryPrefix, this.configuration["Cloudinary:ApiName"]);
        }

        public async Task<IEnumerable<ArenaCardPartialViewModel>> GetAllInCityAsync(int cityId, int? take = null, int skip = 0)
        {
            var query = this.GetAllActiveInCityAsIQueryable(cityId)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var arenas = await query
              .To<ArenaCardPartialViewModel>()
              .ToListAsync();

            foreach (var arena in arenas)
            {
                arena.MainImageUrl = this.SetMainImage(arena.MainImageUrl);
            }

            return arenas;
        }

        public async Task<int> GetCountInCityAsync(int cityId)
        {
            return await this.GetAllActiveInCityAsIQueryable(cityId).CountAsync();
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

        public async Task<T> GetByIdAsync<T>(int id)
        {
            return await this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetIdByAdminIdAsync(string arenaAdminId)
        {
            var arenaId = await this.arenasRepository
                .All()
                .Where(a => a.ArenaAdminId == arenaAdminId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (arenaId == 0)
            {
                throw new ArgumentException(string.Format(UserWithoutArenaErrorMessage, arenaAdminId));
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

            var sportName = await this.sportsService.GetNameByIdAsync(inputModel.SportId);
            await this.emailSender.SendEmailAsync(
                        userEmail,
                        EmailSubjectConstants.ArenaCreated,
                        EmailHtmlMessages.GetArenaCreationHtml(
                            username,
                            inputModel.Name,
                            sportName));
        }

        public async Task<T> GetDetailsAsync<T>(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);

            return await query.To<T>().FirstOrDefaultAsync();
        }

        public async Task<ArenaEditViewModel> GetDetailsForEditAsync(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);

            if (query == null)
            {
                return null;
            }

            var viewModel = await query.To<ArenaEditViewModel>().FirstOrDefaultAsync();
            viewModel.Sports = await this.sportsService.GetAllAsSelectListAsync();

            return viewModel;
        }

        public async Task UpdateAsync(ArenaEditViewModel inputModel)
        {
            var arena = await this.GetArenaByIdAsync(inputModel.Id);

            arena.Name = inputModel.Name;
            arena.PhoneNumber = inputModel.PhoneNumber;
            arena.PricePerHour = inputModel.PricePerHour;
            arena.Description = inputModel.Description;
            arena.SportId = inputModel.SportId;
            arena.WebUrl = inputModel.WebUrl;
            arena.Email = inputModel.Email;
            arena.Status = inputModel.Status;
            arena.Address = inputModel.Address;

            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

        public async Task<ArenaIndexListViewModel> FilterAsync(int countryId, int? sportId, int? cityId, int? take = null, int skip = 0)
        {
            var query = this.GetAllActiveInCountryAsIQueryable<ArenaCardPartialViewModel>(countryId);

            if (sportId != null)
            {
                query = query.Where(a => a.SportId == sportId);
            }

            if (cityId != null)
            {
                query = query.Where(a => a.CityId == cityId);
            }

            var resultCount = query.Count();
            IEnumerable<SelectListItem> sports;

            if (cityId == null || resultCount == 0)
            {
                sports = await this.sportsService.GetAllInCountryByIdAsync(countryId);
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

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = query.ToList(),
                ResultCount = resultCount,
                CityId = cityId,
                SportId = sportId,
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = await this.citiesService.GetAllWithArenasInCountryAsync(countryId),
                    Sports = sports,
                },
            };

            var countryName = await this.countriesService.GetNameByIdAsync(countryId);
            viewModel.Location = cityId.HasValue
              ? await this.citiesService.GetNameByIdAsync(cityId.Value) + ", " + countryName
              : countryName;

            foreach (var model in viewModel.Arenas)
            {
                model.MainImageUrl = this.SetMainImage(model.MainImageUrl);
            }

            return viewModel;
        }

        // images methods
        public string SetMainImage(string imageUrl)
        {
            var resultUrl = DefaultMainImagePath;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                var imagePath = this.imagesService.ConstructUrlPrefix(MainImageSizing);
                resultUrl = imagePath + imageUrl;
            }

            return resultUrl;
        }

        public async Task ChangeMainImageAsync(int arenaId, IFormFile newMainImageFile)
        {
            var arena = await this.GetArenaByIdAsync(arenaId);
            var mainImageId = arena.MainImageId;
            var newMainImage = await this.imagesService.CreateAsync(newMainImageFile);
            arena.MainImageId = newMainImage.Id;
            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();

            if (mainImageId != null)
            {
                await this.imagesService.DeleteByIdAsync(mainImageId);
            }
        }

        public async Task DeleteMainImageAsync(int arenaId)
        {
            var arena = await this.GetArenaByIdAsync(arenaId);
            var mainImageId = arena.MainImageId;
            arena.MainImageId = null;
            this.arenasRepository.Update(arena);
            await this.arenasRepository.SaveChangesAsync();
            await this.imagesService.DeleteByIdAsync(mainImageId);
        }

        public async Task<ArenaImagesEditViewModel> GetImagesByIdAsync(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);

            if (query == null)
            {
                return null;
            }

            var viewModel = await query.To<ArenaImagesEditViewModel>().FirstOrDefaultAsync();

            foreach (var image in viewModel.Images)
            {
                image.Url = this.imagePathPrefix + ImageSizing + image.Url;
            }

            return viewModel;
        }

        public async Task AddImagesAsync(IEnumerable<IFormFile> newImages, int arenaId)
        {
            var arena = await this.GetArenaByIdAsync(arenaId);

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

        public async Task<IEnumerable<string>> GetImageUrslByIdAsync(int id)
        {
            var shortenedUrls = await this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .Select(a => a.Images
                    .Select(i => i.Url)
                    .ToList())
                .FirstOrDefaultAsync();

            var urls = this.imagesService.ConstructUrls(ImageSizing, shortenedUrls);

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

        public async Task<IEnumerable<T>> GetAllInCountryAsync<T>(int countryId, int? take = null, int skip = 0)
        {
            var query = this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .OrderBy(a => a.City.Name)
                .ThenBy(a => a.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query
              .To<T>()
              .ToListAsync();
        }

        public async Task<int> GetCountInCountryAsync(int countryId)
        {
            return await this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .CountAsync();
        }

        public async Task<IndexViewModel> AdminFilterAsync(int countryId, int? cityId, int? sportId, int? take = null, int skip = 0)
        {
            IQueryable<Arena> query = this.arenasRepository
                 .All()
                 .Where(a => a.CountryId == countryId)
                 .OrderBy(c => c.City.Name)
                 .ThenBy(c => c.Name);

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

            var resultCount = await query.CountAsync();

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take.HasValue && resultCount > take)
            {
                query = query.Take(take.Value);
            }

            var arenas = await query
                 .To<InfoViewModel>()
                 .ToListAsync();

            var countryName = await this.countriesService.GetNameByIdAsync(countryId);
            var location = cityId != null
                ? await this.citiesService.GetNameByIdAsync(cityId.Value) + ", " + countryName
                : countryName;

            var viewModel = new IndexViewModel
            {
                ResultCount = resultCount,
                CountryId = countryId,
                CityId = cityId,
                SportId = sportId,
                Arenas = arenas,
                Location = location,
                Filter = new FilterBarViewModel
                {
                    CityId = cityId,
                    SportId = sportId,
                    Cities = await this.citiesService.GetAllInCountryByIdAsync(countryId),
                    Sports = await this.sportsService.GetAllInCountryByIdAsync(countryId),
                },
            };

            return viewModel;
        }

        public async Task AdminUpdateAsync(EditViewModel inputModel)
        {
            var arena = await this.GetArenaByIdAsync(inputModel.Id);

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

        // Helpers
        private async Task<Arena> GetArenaByIdAsync(int arenaId)
        {
            var arena = await this.arenasRepository
            .All()
            .FirstOrDefaultAsync(a => a.Id == arenaId);

            if (arena == null)
            {
                throw new ArgumentException(string.Format(ArenaInvalidIdErrorMessage, arenaId));
            }

            return arena;
        }

        private IQueryable GetArenaByIdAsIQueryable(int arenaId)
        {
            return this.arenasRepository.All()
                .Where(a => a.Id == arenaId);
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
