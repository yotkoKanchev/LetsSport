﻿namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Reports;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Area("Administration")]
    public class ReportsController : Controller
    {
        private readonly IReportsService reportsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;

        public ReportsController(IReportsService reportsService, UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            this.reportsService = reportsService;
            this.userManager = userManager;
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                Reports = await this.reportsService.GetAllAsync<InfoViewModel>(),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var report = await this.reportsService.GetReportByIdAsync<DetailsViewModel>(id.Value);

            if (report == null)
            {
                return this.NotFound();
            }

            return this.View(report);
        }

        public async Task<IActionResult> Block(int id, string reportedUserId, string senderId)
        {
            await this.usersService.BlockUserAsync(reportedUserId);
            await this.reportsService.ArchiveReportAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Refuse(int id, string reportedUserId, string senderId)
        {
            await this.reportsService.ArchiveReportAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Warning(int id, string reportedUserId, string senderId)
        {
            await this.reportsService.ArchiveReportAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}