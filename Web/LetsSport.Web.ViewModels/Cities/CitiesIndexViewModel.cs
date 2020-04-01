namespace LetsSport.Web.ViewModels.Cities
{
    using System.Collections.Generic;

    public class CitiesIndexViewModel
    {
        public IEnumerable<CityInfoViewModel> Cities { get; set; }

        public CitiesFilterBarViewModel Filter { get; set; }
    }
}
