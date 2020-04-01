namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICountriesService
    {
        int GetCountryId(string countryName);

        public IEnumerable<SelectListItem> GetAll();

        int GetCountryIdByArenaId(int arenaId);

        string GetCountryNameById(int countryId);
    }
}
