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

        public IEnumerable<T> GetAllInCountry<T>(int countryId)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .OrderBy(a => a.City.Name)
                .ThenBy(a => a.Name)
                .To<T>();

            return arenas;
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

        public ArenaEditViewModel GetArenaForEdit(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);

            var viewModel = query.To<ArenaEditViewModel>().FirstOrDefault();

            viewModel.Sports = this.sportsService.GetAll();
            return viewModel;
        }

        public async Task UpdateArenaAsync(ArenaEditViewModel viewModel)
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

        public T GetDetails<T>(int id)
        {
            var viewModel = this.GetArenaByIdAsIQueryable(id).To<T>().FirstOrDefault();

            return viewModel;
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

        public ArenaImagesEditViewModel GetArenaImagesByArenaId(int id)
        {
            var query = this.GetArenaByIdAsIQueryable(id);
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

        public IEnumerable<SelectListItem> GetAllArenas((string City, string Country) location)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.Status == ArenaStatus.Active)
                .Where(a => a.City.Name == location.City)
                .Where(c => c.Country.Name == location.Country)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                })
                .ToList();
        }

        public IEnumerable<T> GetAllInCity<T>((string City, string Country) location)
        {
            var query = this.arenasRepository
                .All()
                .Where(a => a.Status == ArenaStatus.Active)
                .Where(a => a.City.Name == location.City)
                .Where(c => c.Country.Name == location.Country)
                .OrderBy(a => a.Name);

            var arenas = query.To<T>();

            return arenas;
        }

        public IndexViewModel FilterArenasByCountryId(int countryId)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.CountryId == countryId)
                .OrderBy(a => a.City.Name)
                .ThenBy(a => a.Name)
                .To<InfoViewModel>()
                .ToList();

            var arenaName = this.countriesService.GetCountryNameById(countryId);

            var viewModel = new IndexViewModel
            {
                Arenas = arenas,
                Filter = new FilterBarViewModel
                {
                    Cities = this.citiesService.GetCitiesInCountryById(countryId),
                    Sports = this.sportsService.GetAllSportsInCountryById(countryId),
                },
            };

            return viewModel;
        }

        public IndexViewModel FilterArenas(int countryId, int? cityId, int? sportId, int? isDeleted)
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

            var arenaName = this.countriesService.GetCountryNameById(countryId);
            var countryName = this.countriesService.GetCountryNameById(countryId);
            var location = cityId != null
                ? this.citiesService.GetCityNameById(cityId.Value) + ", " + countryName
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
                    Cities = this.citiesService.GetCitiesInCountryById(countryId),
                    Sports = this.sportsService.GetAllSportsInCountryById(countryId),
                },
            };

            return viewModel;
        }

        public bool IsArenaExists(string userId) => this.GetArenaIdByAdminId(userId) > 0;

        public ArenaIndexListViewModel FilterArenas(string country, int sport, int city)
        {
            var query = this.GetAllInCountry<ArenaCardPartialViewModel>(country);

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
                sports = this.sportsService.GetAllSportsByCountryName(country);
            }
            else
            {
                var sportsHash = new HashSet<SelectListItem>();

                foreach (var sportKvp in query)
                {
                    sportsHash.Add(new SelectListItem
                    {
                        Text = sportKvp.SportName,
                        Value = sportKvp.SportId.ToString(),
                    });
                }

                sports = sportsHash;
            }

            var viewModel = new ArenaIndexListViewModel
            {
                Arenas = query.ToList(),
                Filter = new FilterBarArenasPartialViewModel
                {
                    Cities = this.citiesService.GetCitiesWithArenas(country),
                    Sports = sports,
                },
            };

            return viewModel;
        }

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

        public IEnumerable<SelectListItem> GetAllArenasInCitySelectList(int? cityId)
        {
            var arenas = this.arenasRepository
                .All()
                .Where(a => a.CityId == cityId)
                .ToList();

            return arenas.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString(),
            });
        }

        public bool CheckUserIsArenaAdmin(string id)
        {
            return this.arenasRepository
                .All()
                .Any(a => a.ArenaAdminId == id);
        }

        // Admin
        public T GetArenaById<T>(int id)
        {
            return this.arenasRepository
                .All()
                .Where(a => a.Id == id)
                .To<T>()
                .FirstOrDefault();
        }

        public async Task AdminUpdateArenaAsync(EditViewModel inputModel)
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

        public async Task DeleteById(int id)
        {
            var arena = this.arenasRepository
                .All()
                .FirstOrDefault(a => a.Id == id);
            this.arenasRepository.Delete(arena);
            await this.arenasRepository.SaveChangesAsync();
        }

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

        private IEnumerable<T> GetAllInCountry<T>(string country)
        {
            var query = this.arenasRepository
                .All()
                .Where(c => c.Country.Name == country)
                .Where(a => a.Status == ArenaStatus.Active)
                .OrderBy(a => a.Name);

            var arenas = query.To<T>();

            return arenas;
        }
    }
}
