﻿namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.GlobalConstants;
    using static LetsSport.Web.Common.ConfirmationMessages;

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
            var viewModel = await this.reportsService.CreateAsync(reportedUserId, sender.Id, sender.UserName);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Report(string senderUserId, int abuse, string content, string reportedUserId)
        {
            if (!this.ModelState.IsValid)
            {
                var sender = await this.userManager.GetUserAsync(this.User);
                var viewModel = await this.reportsService.CreateAsync(reportedUserId, sender.Id, sender.UserName);

                return this.View(viewModel);
            }

            await this.reportsService.AddAsync(senderUserId, abuse, content, reportedUserId);
            this.TempData[TempDataMessage] = ReportedUser;

            return this.RedirectToAction("Details", "Users", new { id = reportedUserId });
        }
    }
}
