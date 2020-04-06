namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICountriesService
    {
        IEnumerable<SelectListItem> GetAllAsSelectList();

        T GetById<T>(int id);

        int GetId(string countryName);

        string GetNameById(int countryId);

        // admin
        IEnumerable<T> GetAll<T>(int? take = null, int skip = 0);

        int GetCount();

        Task<int> CreateAsync(string name);

        Task UpdateAsync(int id, string name);

        Task DeleteByIdAsync(int id);
    }
}
