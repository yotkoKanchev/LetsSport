namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICountriesService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();

        Task<T> GetByIdAsync<T>(int id);

        Task<int> GetIdAsync(string countryName);

        Task<string> GetNameByIdAsync(int countryId);

        // admin
        Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0);

        Task<int> GetCountAsync();

        Task<int> CreateAsync(string name);

        Task UpdateAsync(int id, string name);

        Task DeleteByIdAsync(int id);
    }
}
