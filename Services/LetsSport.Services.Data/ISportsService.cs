namespace LetsSport.Services.Data
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        IEnumerable<SelectListItem> GetAll();

        IEnumerable<SelectListItem> GetAllSportsInCountry(string countryName);

        HashSet<string> GetAllSportsInCurrentCountry(string currentCountry);

        int GetSportId(string sport);

        string GetSportNameById(int? sportId);

        string GetSportImageByName(string sport);
    }
}
