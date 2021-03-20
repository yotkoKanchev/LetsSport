namespace LetsSport.Web.Tests.Controllers
{
    using LetsSport.Web.Controllers;
    using LetsSport.Web.ViewModels.ArenaRequests;
    using Microsoft.AspNetCore.Mvc;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    public class ArenaRequestsControllerTests : BaseControllerTests
    {
        //[Fact]
        //public void ChangeStatusShouldReturnView()
        //{
        //    var model = new RequestViewModel { EventId = 1, EventInfo = new EventInfoViewModel { }, Id = "id", };
        //    MyController<ArenaRequestsController>
        //        .Instance()
        //        .Calling(c => c.ChangeStatus("id"))
        //        .ShouldReturn()
        //        .View()
        //        .AndAlso()
        //        .ShouldPassForThe<ViewResult>(viewResult =>
        //            Assert.Same(model, viewResult.Model));
        //}

        //[Fact]
        //public void AccountControllerShouldHaveAuthorizeFilter()
        //{
        //    MyController<ArenaRequestsController>
        //        .Instance()
        //        .ShouldHave()
        //        .Attributes(attrs => attrs
        //            .RestrictingForAuthorizedRequests());
        //}
    }
}
