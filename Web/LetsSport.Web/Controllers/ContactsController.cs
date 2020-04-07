namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.AspNetCore.Mvc;

    public class ContactsController : BaseController
    {
        private readonly IContactsService contactsService;

        public ContactsController(ILocationLocator locationLocator, IContactsService contactsService)
            : base(locationLocator)
        {
            this.contactsService = contactsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactIndexViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.contactsService.FileContactFormAsync(inputModel);
            var senderName = inputModel.Name;

            return this.RedirectToAction(nameof(this.ThankYou), senderName);
        }

        public IActionResult ThankYou(string name)
        {
            var viewModel = this.contactsService.SayThankYou(name);

            return this.View(viewModel);
        }
    }
}
