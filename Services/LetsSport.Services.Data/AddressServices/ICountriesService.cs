namespace LetsSport.Services.Data.AddressServices
{
    using System.Threading.Tasks;

    public interface ICountriesService
    {
        Task<int> GetCountryIdAsync(string countryName);
    }
}
