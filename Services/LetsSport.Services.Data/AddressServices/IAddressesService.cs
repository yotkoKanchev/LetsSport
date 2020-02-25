namespace LetsSport.Services.Data.AddressServices
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public interface IAddressesService
    {
        [Display(Name = "Create")]
        Task<int> CreateAsync(string country, string city, string addressFromInput);

        Task UpdateAddressAsync(int addresId, string newAddress);
    }
}
