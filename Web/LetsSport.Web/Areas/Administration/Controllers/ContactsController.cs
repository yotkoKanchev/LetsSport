namespace LetsSport.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Data.Contacts;
    using LetsSport.Web.ViewModels.Admin.Contacts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using static LetsSport.Common.ConfirmationMessages;
    using static LetsSport.Common.GlobalConstants;

    [Authorize(Roles = AdministratorRoleName)]
    [Area(AdministrationAreaName)]
    public class ContactsController : Controller
    {
        private const int ItemsPerPage = AdminLargeItemsPerPageCount;
        private readonly IContactsService contactsService;
        private readonly IRepository<ContactForm> contactFormsRepository;

        public ContactsController(IContactsService contactsService, IRepository<ContactForm> contactFormsRepository)
        {
            this.contactsService = contactsService;
            this.contactFormsRepository = contactFormsRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = new IndexViewModel
            {
                Messages = await this.contactsService.GetAllAsync<ContactInfoViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
            };

            var count = await this.contactsService.GetCountAsync();
            viewModel.CurrentPage = page;
            viewModel.PageCount = (int)Math.Ceiling((double)count / ItemsPerPage) != 0
                ? (int)Math.Ceiling((double)count / ItemsPerPage) : 1;

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var report = await this.contactsService.GetByIdAsync<DetailsViewModel>(id.Value);

            if (report == null)
            {
                return this.NotFound();
            }

            return this.View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Reply([Bind("ReplyContent, Id")]DetailsViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var recipientEmail = this.GetSenderEmail(inputModel.Id);
            await this.contactsService.ReplyAsync(inputModel.Id, inputModel.ReplyContent, recipientEmail);
            this.TempData[TempDataMessage] = MessageReplyed;

            return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> Ignore(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            await this.contactsService.IgnoreAsync(id.Value);
            this.TempData[TempDataMessage] = MessageIgnored;

            return this.RedirectToAction(nameof(this.Index));
        }

        private string GetSenderEmail(int id)
        {
            return this.contactFormsRepository.All()
                .Where(cf => cf.Id == id)
                .Select(cf => cf.Email)
                .FirstOrDefault();
        }
    }
}
