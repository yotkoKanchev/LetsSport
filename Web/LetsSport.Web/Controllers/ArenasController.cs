namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.AddressServices;
    using LetsSport.Web.ViewModels.Arenas;
    using LetsSport.Services.Data.Common;
    using Microsoft.AspNetCore.Mvc;

    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly IAddressesService addressesService;
        private readonly ICitiesService citiesService;
        private readonly Cloudinary cloudinary;

        public ArenasController(
            IArenasService arenasService,
            IAddressesService addressesService,
            ICitiesService citiesService,
            Cloudinary cloudinary)
        {
            this.arenasService = arenasService;
            this.addressesService = addressesService;
            this.citiesService = citiesService;
            this.cloudinary = cloudinary;
        }

        public async Task<IActionResult> Create()
        {
            // TODO pass filtered cities per country
            // TODO add current country as default
            var cities = await this.citiesService.GetCitiesAsync();
            this.ViewData["cities"] = cities;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var cities = await this.citiesService.GetCitiesAsync();
                this.ViewData["cities"] = cities;

                return this.View(inputModel);
            }

            var arenaId = await this.arenasService.CreateAsync(inputModel);
            var picturesResult = await ApplicationCloudinary.UploadFilesAsync(this.cloudinary, inputModel.Pictures, inputModel.Name);
            var profilePictureResult = await ApplicationCloudinary.UploadFileAsync(this.cloudinary, inputModel.ProfilePicture, inputModel.Name);
            // TODO pass filtered by sport Arenas with AJAX;
            return this.Redirect($"details/{arenaId}");
        }

        public IActionResult Details(int id)
        {
            var inputModel = this.arenasService.GetArena(id);

            return this.View(inputModel);
        }

        public IActionResult Edit(int id)
        {
            var inputModel = this.arenasService.GetArenaForEdit(id);

            return this.View(inputModel);
        }

        [HttpPost]
        public IActionResult Edit(ArenaEditViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                var inputModel = this.arenasService.GetArenaForEdit(viewModel.Id);

                return this.View(inputModel);
            }

            this.arenasService.UpdateArenaAsync(viewModel);

            var arenaId = viewModel.Id;
            return this.Redirect($"/arenas/details/{arenaId}");
        }
    }
}
