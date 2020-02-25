namespace LetsSport.Services.Data.AddressServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models.AddressModels;

    public class AddressesService : IAddressesService
    {
        private readonly IDeletableEntityRepository<Address> addressesRepository;
        private readonly ICitiesService citiesService;

        public AddressesService(
            IDeletableEntityRepository<Address> addressesRepository,
            ICitiesService citiesService)
        {
            this.addressesRepository = addressesRepository;
            this.citiesService = citiesService;
        }

        public async Task<int> CreateAsync(string country, string city, string addressFromInput)
        {
            var cityId = await this.citiesService.GetCityIdAsync(city, country);

            var address = new Address
            {
                CityId = cityId,
                StreetAddress = addressFromInput,
                CreatedOn = DateTime.UtcNow,
            };

            await this.addressesRepository.AddAsync(address);
            await this.addressesRepository.SaveChangesAsync();

            return address.Id;
        }

        public void UpdateAddress(int addresId, string newAddress)
        {
            var address = this.addressesRepository
                .All()
                .Where(a => a.Id == addresId)
                .First();

            if (address.StreetAddress != newAddress)
            {
                address.StreetAddress = newAddress;
                this.addressesRepository.Update(address);
            }
        }
    }
}
