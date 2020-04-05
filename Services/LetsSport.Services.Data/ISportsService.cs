namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        IEnumerable<T> GetAll<T>();

        IEnumerable<SelectListItem> GetAllAsSelectList();

        Task<IEnumerable<SelectListItem>> GetAllSportsInCountryByIdAsync(int countryId);

        IEnumerable<SelectListItem> GetAllSportsInCityById(int? cityId);

        string GetSportNameById(int? sportId);

        string GetSportImageByName(string sport);

        // Administration
        Task<int> CreateSport(string name, string image);

        T GetSportById<T>(int id);

        Task UpdateSport(int id, string name, string image);

        Task DeleteById(int id);
    }
}
