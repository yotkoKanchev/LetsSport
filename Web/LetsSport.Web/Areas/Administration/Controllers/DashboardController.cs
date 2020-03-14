﻿namespace LetsSport.Web.Areas.Administration.Controllers
{
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService, ILocationLocator locationLocator)
            : base(locationLocator)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            return this.View(viewModel);
        }
    }
}
