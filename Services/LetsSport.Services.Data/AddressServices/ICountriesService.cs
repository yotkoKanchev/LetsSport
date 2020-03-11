namespace LetsSport.Services.Data.AddressServices
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public interface ICountriesService
    {
        int GetCountryId(string countryName);

        public IEnumerable<SelectListItem> GetAll();
    }
}
