namespace LetsSport.Services.Data.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Admin.Contacts;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ContactsServiceTests : BaseServiceTests
    {
        private IContactsService Service => this.ServiceProvider.GetRequiredService<IContactsService>();

        [Fact]
        public async Task FileContactFormAsyncShouldCreateNewContactFormInDb()
        {
            var formCount = this.DbContext.ContactForms.ToArray().Count();
            Assert.Equal(0, formCount);

            var viewModel = new ContactIndexViewModel
            {
                Content = "test",
                Email = "test@email.com",
                Name = "TestName",
                Title = "TestTitle",
            };

            await this.Service.FileContactFormAsync(viewModel);
            var form = await this.DbContext.ContactForms.FirstOrDefaultAsync();
            formCount = this.DbContext.ContactForms.ToArray().Count();

            Assert.Equal(1, formCount);
            Assert.Equal("test", form.Content);
            Assert.Equal("test@email.com", form.Email);
            Assert.Equal("TestName", form.Name);
            Assert.Equal("TestTitle", form.Title);
        }

        [Fact]
        public void SayThankYouShuldReturnCorrectViewModel()
        {
            var name = "TestName";
            Assert.IsType<ContactTankYouViewModel>(this.Service.SayThankYou(name));
            var view = this.Service.SayThankYou(name);
            Assert.Equal("TestName", view.SenderName);
        }

        [Fact]
        public async Task GetCountAsyncReturnCorrectResult()
        {
            var formCount = await this.Service.GetCountAsync();
            Assert.Equal(0, formCount);

            var viewModel = new ContactIndexViewModel
            {
                Content = "test",
                Email = "test@email.com",
                Name = "TestName",
                Title = "TestTitle",
            };

            await this.Service.FileContactFormAsync(viewModel);
            formCount = await this.Service.GetCountAsync();

            Assert.Equal(1, formCount);
        }

        [Fact]
        public async Task GetByIdAsyncReturnsCorrectRecord()
        {
            var viewModel = new ContactIndexViewModel
            {
                Content = "test",
                Email = "test@email.com",
                Name = "TestName",
                Title = "TestTitle",
            };

            var id = await this.Service.FileContactFormAsync(viewModel);
            var form = await this.Service.GetByIdAsync<ContactIndexViewModel>(id);

            Assert.Equal("test", form.Content);
            Assert.Equal("test@email.com", form.Email);
            Assert.Equal("TestName", form.Name);
            Assert.Equal("TestTitle", form.Title);
        }

        [Fact]
        public async Task GetAllAsyncShouldReturnCorrectResult()
        {
            var result = await this.Service.GetAllAsync<ContactInfoViewModel>();
            Assert.Single(this.DbContext.ContactForms);
        }

        [Fact]
        public async Task ReplyAsyncShouldSetIsReplyedTrue()
        {
            var form = this.DbContext.ContactForms.FirstOrDefault();
            Assert.False(form.IsReplyed);
            Assert.Null(form.ReplyContent);

            var replyContent = "Reply content with more than 20 characters";
            await this.Service.ReplyAsync(1, replyContent, "secondemail@abv.bg");
            form = this.DbContext.ContactForms.FirstOrDefault();
            Assert.True(form.IsReplyed);
            Assert.Equal("Reply content with more than 20 characters", form.ReplyContent);
        }

        [Fact]
        public async Task IgnoreAsyncShouldSetIGNOREReplyContent()
        {
            var newForm = new ContactForm
            {
                Content = "This contact form content should be more than 20 chars",
                Email = "test@email.com",
                Name = "Test Tetov",
                Title = "Test title",
            };

            this.DbContext.ContactForms.Add(newForm);
            await this.DbContext.SaveChangesAsync();
            Assert.False(newForm.IsReplyed);
            Assert.Null(newForm.ReplyContent);

            await this.Service.IgnoreAsync(2);
            newForm = this.DbContext.ContactForms.FirstOrDefault(cf => cf.Id == 2);
            Assert.True(newForm.IsReplyed);
            Assert.Equal("IGNORED", newForm.ReplyContent);
        }
    }
}
