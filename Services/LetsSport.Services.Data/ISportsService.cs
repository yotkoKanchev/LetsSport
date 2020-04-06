namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        IEnumerable<SelectListItem> GetAllAsSelectList();

        Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId);

        IEnumerable<SelectListItem> GetAllInCityById(int? cityId);

        string GetNameById(int? sportId);

        string GetImageByName(string sport);

        // Administration
        IEnumerable<T> GetAll<T>(int? take = null, int skip = 0);

        Task<int> AddAsync(string name, string image);

        T GetById<T>(int id);

        Task UpdateAsync(int id, string name, string image);

        Task DeleteByIdAsync(int id);

        int GetCount();
    }
}
