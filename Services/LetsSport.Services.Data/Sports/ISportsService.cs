namespace LetsSport.Services.Data.Sports
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ISportsService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();

        Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId);

        Task<IEnumerable<SelectListItem>> GetAllInCityByIdAsync(int? cityId);

        Task<string> GetNameByIdAsync(int? sportId);

        Task<string> GetImageByNameAsync(string sport);

        // Administration
        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<int> CreateAsync(string name, string image);

        Task<T> GetByIdAsync<T>(int id);

        Task UpdateAsync(int id, string name, string image);

        Task DeleteByIdAsync(int id);

        Task<int> GetCountAsync();
    }
}
