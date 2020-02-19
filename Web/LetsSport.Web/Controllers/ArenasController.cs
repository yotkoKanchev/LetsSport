namespace LetsSport.Web.Controllers
{
    using LetsSport.Data.Models.AddressModels;
    using Microsoft.AspNetCore.Mvc;

    public class ArenasController : BaseController
    {
        public IActionResult Create()
        {
            // TODO pass filtered cities per country
            // TODO add current country as default
            return this.View();
        }
    }
}
