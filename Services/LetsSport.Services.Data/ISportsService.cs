namespace LetsSport.Services.Data
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        IEnumerable<SelectListItem> GetAll();

        IEnumerable<SelectListItem> GetAllSportsInCountry(string countryName);

        IEnumerable<SelectListItem> GetAllSportsInCountryById(int countryId);

        IEnumerable<SelectListItem> GetAllSportsInCity(int? cityId);

        string GetSportNameById(int? sportId);

        string GetSportImageByName(string sport);
    }
}
