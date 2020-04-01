﻿namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SportsService : ISportsService
    {
        private readonly IRepository<Sport> sportsRepository;

        public SportsService(IRepository<Sport> sportsRepository)
        {
            this.sportsRepository = sportsRepository;
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var sports = this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                });

            return sports;
        }

        public IEnumerable<SelectListItem> GetAllSportsInCountry(string countryName)
        {
            return this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.Country.Name == countryName))
                .Distinct()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToList();
        }

        public IEnumerable<SelectListItem> GetAllSportsInCountryById(int countryId)
        {
            return this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.Country.Id == countryId))
                .Distinct()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToList();
        }

        public IEnumerable<SelectListItem> GetAllSportsInCity(int? cityId)
        {
            return this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.CityId == cityId))
                .Distinct()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToList();
        }

        public string GetSportImageByName(string sport)
        {
            return this.sportsRepository
                .All()
                .Where(s => s.Name == sport)
                .Select(s => s.Image)
                .FirstOrDefault();
        }

        public string GetSportNameById(int? sportId)
        {
            return this.sportsRepository
                .All()
                .Where(s => s.Id == sportId)
                .Select(s => s.Name)
                .FirstOrDefault();
        }
    }
}
