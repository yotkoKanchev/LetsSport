﻿namespace LetsSport.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models.AddressModels;

    public interface IAddressesService
    {
        Task<int> Create(string country, string city, string addressFromInput);

        Task<IEnumerable<string>> GetCities();

        void UpdateAddress(int addresId, string newAddress);
    }
}
