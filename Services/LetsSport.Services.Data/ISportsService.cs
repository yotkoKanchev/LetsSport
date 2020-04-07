namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();

        Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId);

        Task<IEnumerable<SelectListItem>> GetAllInCityByIdAsync(int? cityId);

        string GetNameById(int? sportId);

        string GetImageByName(string sport);

        // Administration
        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<int> AddAsync(string name, string image);

        T GetById<T>(int id);

        Task UpdateAsync(int id, string name, string image);

        Task DeleteByIdAsync(int id);

        int GetCount();
    }
}
