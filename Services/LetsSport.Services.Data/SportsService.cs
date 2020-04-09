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

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
        {
            var sports = await this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();

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

        public async Task<IEnumerable<SelectListItem>> GetAllInCityByIdAsync(int? cityId)
        {
            return await this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.CityId == cityId))
                .Distinct()
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();
        }

        public async Task<string> GetImageByNameAsync(string sport)
        {
            var result = await this.sportsRepository
                .All()
                .Where(s => s.Name == sport)
                .Select(s => s.Image)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<string> GetNameByIdAsync(int? sportId)
        {
            return await this.GetAsIQueryable(sportId.Value)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();
        }

        // Admin
        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.To<T>()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            var sport = await this.GetAsIQueryable(id).FirstAsync();

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
            var sport = await this.GetAsIQueryable(id).FirstAsync();

            sport.Name = name;
            sport.Image = image;

            this.sportsRepository.Update(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var sport = await this.GetAsIQueryable(id).FirstAsync();
            this.sportsRepository.Delete(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await this.sportsRepository.All().CountAsync();
        }

        private IQueryable<Sport> GetAsIQueryable(int id)
        {
            var query = this.sportsRepository
                .All()
                .Where(s => s.Id == id);

            if (!query.Any())
            {
                throw new ArgumentException($"Sport with ID: {id} does not exists!");
            }

            return query;
        }
    }
}
