namespace LetsSport.Web.Tests.Controllers
{
    using LetsSport.Web.Controllers;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class AboutControllerTests
    {
        [Fact]
        public void IndexShouldReturnView()
            =>
            MyController<AboutController>
                .Instance()
                .Calling(c => c.Index())
                .ShouldReturn()
                .View();
    }
}
