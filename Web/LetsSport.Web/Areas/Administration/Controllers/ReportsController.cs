namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data;
    using LetsSport.Web.ViewModels.Admin.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area(GlobalConstants.AdministrationAreaName)]
    public class ReportsController : Controller
    {
        private const int ItemsPerPage = 20;
        private readonly IReportsService reportsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService usersService;

        public ReportsController(IReportsService reportsService, UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            this.reportsService = reportsService;
            this.userManager = userManager;
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                Reports = await this.reportsService.GetAllAsync<InfoViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.reportsService.GetCountAsync();
            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }

        public async Task<IActionResult> Filter(int deletionStatus, int page = 1)
        {
            var viewModel = await this.reportsService.FilterAsync(deletionStatus, ItemsPerPage, (page - 1) * ItemsPerPage);

            if (viewModel == null)
            {
                return this.NotFound();
            }

            var count = viewModel.ResultCount;
            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(nameof(this.Index), viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var report = await this.reportsService.GetByIdAsync<DetailsViewModel>(id.Value);

            if (report == null)
            {
                return this.NotFound();
            }

            return this.View(report);
        }

        public async Task<IActionResult> Block(int id, string reportedUserId, string senderId)
        {
            await this.usersService.BlockUserAsync(reportedUserId);
            await this.reportsService.ArchiveAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Refuse(int id, string reportedUserId, string senderId)
        {
            await this.reportsService.ArchiveAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }

        public async Task<IActionResult> Warning(int id, string reportedUserId, string senderId)
        {
            await this.reportsService.ArchiveAsync(id);

            // TODO send 2 emails
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
