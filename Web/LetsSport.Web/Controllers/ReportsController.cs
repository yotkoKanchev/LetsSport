namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ReportsController : BaseController
    {
        private readonly IReportsService reportsService;
        private readonly UserManager<ApplicationUser> userManager;

        public ReportsController(
            IReportsService reportsService,
            UserManager<ApplicationUser> userManager,
            ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.reportsService = reportsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Report(string reportedUserId)
        {
            var sender = await this.userManager.GetUserAsync(this.User);
            var viewModel = this.reportsService.Create(reportedUserId, sender.Id, sender.UserName);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Report(string senderUserId, int abuse, string content, string reportedUserId)
        {
            if (!this.ModelState.IsValid)
            {
                var sender = await this.userManager.GetUserAsync(this.User);
                var viewModel = this.reportsService.Create(reportedUserId, sender.Id, sender.UserName);

                return this.View(viewModel);
            }

            await this.reportsService.AddAsync(senderUserId, abuse, content, reportedUserId);

            return this.RedirectToAction("Details", "Users", new { id = reportedUserId });
        }
    }
}
