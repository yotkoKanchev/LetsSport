namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;

    public interface ICountriesService
    {
        int GetCountryId(string countryName);

        public IEnumerable<string> GetAll();
    }
}
