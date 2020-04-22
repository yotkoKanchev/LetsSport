namespace LetsSport.Web.Tests.Controllers
{
    using System;

    using LetsSport.Web.Controllers;
    using LetsSport.Web.ViewModels.Contacts;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class ContactsControllerTests : BaseControllerTests
    {
        [Fact]
        public void IndexShouldReturnView()
        {
            MyController<ContactsController>
                .Instance()
                .Calling(c => c.Index())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void IndexShouldRedirectWithValidModelState()
        {
            var inputModel = new ContactIndexViewModel
            {
                Content = "Test Content should be more than 20 characters",
                Email = "test@test.com",
                Name = "Test Testov",
                Title = "Test Title",
                RecaptchaValue = Guid.NewGuid().ToString(),
            };

            MyController<ContactsController>
            .Instance()
            .Calling(c => c.Index(inputModel))
            .ShouldHave()
            .ModelState(ms => ms.ContainingNoErrors())
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("ThankYou");
        }

        [Fact]
        public void ThankYouShouldReturnView()
        {
            var name = "Sender Name";
            var viewModel = new ContactTankYouViewModel() { SenderName = name };
            MyController<ContactsController>
                .Instance()
                .Calling(c => c.ThankYou(name))
                .ShouldReturn()
                .View(viewModel);
        }
    }
}
