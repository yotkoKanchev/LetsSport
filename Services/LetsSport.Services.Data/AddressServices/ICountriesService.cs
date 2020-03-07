using System.Collections.Generic;

namespace LetsSport.Services.Data.AddressServices
{
    public interface ICountriesService
    {
        int GetCountryId(string countryName);

        public IEnumerable<string> GetAll();
    }
}
