﻿namespace LetsSport.Services.Data.AddressServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LetsSport.Data.Models;
    using LetsSport.Web.ViewModels.Cities;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICitiesService
    {
        IEnumerable<City> GetAll();

        Task CreateCityAsync(string cityName, int countryId);

        IEnumerable<SelectListItem> GetCitiesSelectList(int countryId);

        Task<int> GetCityIdAsync((string CityName, string Country) location);

        Task<IEnumerable<SelectListItem>> GetCitiesAsync((string City, string Country) location);

        IEnumerable<SelectListItem> GetCitiesWithEventsAsync(string country);

        bool IsCityExists(string cityName, int countryId);

        IList<SelectListItem> GetCitiesWithArenas(string country);

        string GetLocationByCityId(int cityId);

        int GetCityIdByArenaId(int arenaId);

        string GetCityNameById(int cityId);

        CitiesIndexViewModel FilterCities(int? country, int status);
    }
}
