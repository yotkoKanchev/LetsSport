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
    using Microsoft.EntityFrameworkCore;

    public class SportsService : ISportsService
    {
        private readonly IRepository<Sport> sportsRepository;

        public SportsService(IRepository<Sport> sportsRepository)
        {
            this.sportsRepository = sportsRepository;
        }

        public IEnumerable<SelectListItem> GetAllAsSelectList()
        {
            var sports = this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToList();

            return sports;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId)
        {
            return await this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.Country.Id == countryId))
                .Distinct()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();
        }

        public IEnumerable<SelectListItem> GetAllInCityById(int? cityId)
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

        public string GetImageByName(string sport)
        {
            return this.sportsRepository
                .All()
                .Where(s => s.Name == sport)
                .Select(s => s.Image)
                .FirstOrDefault();
        }

        public string GetNameById(int? sportId)
        {
            return this.GetAsIQueryable(sportId.Value)
                .Select(s => s.Name)
                .FirstOrDefault();
        }

        // Admin
        public IEnumerable<T> GetAll<T>(int? take = null, int skip = 0)
        {
            var query = this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>()
                .ToList();
        }

        public T GetById<T>(int id)
        {
            var sport = this.GetAsIQueryable(id).FirstOrDefault();

            return sport.To<T>();
        }

        public async Task<int> AddAsync(string name, string image)
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

        public async Task UpdateAsync(int id, string name, string image)
        {
            var sport = this.GetAsIQueryable(id).FirstOrDefault();

            sport.Name = name;
            sport.Image = image;

            this.sportsRepository.Update(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var sport = this.GetAsIQueryable(id).FirstOrDefault();
            this.sportsRepository.Delete(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public int GetCount()
        {
            return this.sportsRepository.All().Count();
        }

        private IQueryable<Sport> GetAsIQueryable(int id)
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
