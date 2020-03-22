namespace LetsSport.Services.Data
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        IEnumerable<SelectListItem> GetAll();

        IEnumerable<SelectListItem> GetAllSportsInCountry(string countryName);

        int GetSportId(string sport);

        string GetSportNameById(int? sportId);
    }
}
