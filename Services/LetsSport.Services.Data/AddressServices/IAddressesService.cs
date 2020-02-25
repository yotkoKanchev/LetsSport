namespace LetsSport.Services.Data.AddressServices
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public interface IAddressesService
    {
        [Display(Name = "Create")]
        Task<int> CreateAsync(string country, string city, string addressFromInput);

        void UpdateAddress(int addresId, string newAddress);
    }
}
