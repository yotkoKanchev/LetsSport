namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
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

        public IEnumerable<SelectListItem> GetAllSportsByCountryName(string countryName)
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

        public IEnumerable<SelectListItem> GetAllSportsInCityById(int? cityId)
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

        public IEnumerable<T> GetAllAsIQueryable<T>()
        {
            return this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .To<T>()
                .ToList();
        }

        public async Task<int> CreateSport(string name, string image)
        {
            var sport = new Sport
            {
                Name = name,
                Image = image,
            };

            await this.sportsRepository.AddAsync(sport);
            await this.sportsRepository.SaveChangesAsync();

            return sport.Id;
        }

        public T GetSportById<T>(int id)
        {
            var sport = this.GetSportAsIQueryable(id).FirstOrDefault();

            return sport.To<T>();
        }

        public async Task UpdateSport(int id, string name, string image)
        {
            var sport = this.GetSportAsIQueryable(id).FirstOrDefault();

            sport.Name = name;
            sport.Image = image;

            this.sportsRepository.Update(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task DeleteById(int id)
        {
            var sport = this.GetSportAsIQueryable(id).FirstOrDefault();
            this.sportsRepository.Delete(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        private IQueryable<Sport> GetSportAsIQueryable(int id)
        {
            var sport = this.sportsRepository
                .All()
                .Where(s => s.Id == id);

            if (sport == null)
            {
                throw new ArgumentException($"Sport with ID: {id} does not exists!");
            }

            return sport;
        }
    }
}
