namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICountriesService
    {
        int GetCountryId(string countryName);

        public IEnumerable<SelectListItem> GetAll();

        int GetCountryIdByArenaId(int arenaId);

        string GetCountryNameById(int countryId);

        IEnumerable<T> GetAll<T>();

        Task<int> CreateCountry(string name);

        T GetCountryById<T>(int id);

        Task UpdateCountryAsync(int id, string name);

        Task DeleteById(int id);
    }
}
