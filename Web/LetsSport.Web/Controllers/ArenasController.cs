namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Arenas;
    using Microsoft.AspNetCore.Mvc;

    public class ArenasController : BaseController
    {
        private readonly IArenasService arenasService;
        private readonly IAddressesService addressesService;

        public ArenasController(IArenasService arenasService, IAddressesService addressesService)
        {
            this.arenasService = arenasService;
            this.addressesService = addressesService;
        }

        public IActionResult Create()
        {
            // TODO pass filtered cities per country
            // TODO add current country as default
            var cities = this.addressesService.GetCities();
            this.ViewData["cities"] = cities;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArenaCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Error");
            }

            await this.arenasService.CreateAsync(inputModel);

            // TODO pass filtered by sport Arenas with AJAX;
            return this.Redirect("/");
        }

        public IActionResult Details(int id)
        {
            var inputModel = this.arenasService.GetArena(id);

            return this.View(inputModel);
        }
    }
}
