namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
    public class DashboardController : AdministrationController
    {
        private readonly ApplicationDbContext db;

        public DashboardController(ApplicationDbContext db, ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                UsersCount = await this.db.Users.CountAsync(),
                CountriesCount = await this.db.Countries.CountAsync(),
                CitiesCount = await this.db.Cities.CountAsync(),
                SportsCount = await this.db.Sports.CountAsync(),
                EventsCount = await this.db.Events.CountAsync(),
                ArenasCount = await this.db.Arenas.CountAsync(),
                ReportsCount = await this.db.Reports.CountAsync(),
                MessagesCount = await this.db.ContactForms.CountAsync(),
            };

            return this.View(viewModel);
        }
    }
}
