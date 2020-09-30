namespace LetsSport.Web.Controllers
{
    using System.Threading.Tasks;

    using LetsSport.Services.Data.Contacts;
    using LetsSport.Web.Filters;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.AspNetCore.Mvc;

    [ServiceFilter(typeof(SetLocationResourceFilter))]
    public class ContactsController : BaseController
    {
        private readonly IContactsService contactsService;

        public ContactsController(IContactsService contactsService)
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
            var name = inputModel.Name;

            return this.RedirectToAction(nameof(this.ThankYou), new { name });
        }

        public IActionResult ThankYou(string name)
        {
            var viewModel = this.contactsService.SayThankYou(name);

            return this.View(viewModel);
        }
    }
}
