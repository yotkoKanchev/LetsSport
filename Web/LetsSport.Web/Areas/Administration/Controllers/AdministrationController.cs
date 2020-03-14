namespace LetsSport.Web.Areas.Administration.Controllers
{
    using LetsSport.Common;
    using LetsSport.Services.Data.Common;
    using LetsSport.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
        public AdministrationController(ILocationLocator locationLocator)
            : base(locationLocator)
        {
        }
    }
}
