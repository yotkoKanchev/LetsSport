namespace LetsSport.Services.Data.Sports
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

    using static LetsSport.Common.ErrorMessages;

    public class SportsService : ISportsService
    {
        private readonly IRepository<Sport> sportsRepository;

        public SportsService(IRepository<Sport> sportsRepository)
        {
            this.sportsRepository = sportsRepository;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
            => await this.sportsRepository
                .All()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetAllInCountryByIdAsync(int countryId)
            => await this.sportsRepository
                .All()
                .Where(s => s.Arenas
                    .Any(a => a.Country.Id == countryId))
                .Distinct()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();

        public async Task<IEnumerable<SelectListItem>> GetAllInCityByIdAsync(int? cityId)
            => await this.sportsRepository
                .All()
                .Where(s => s.Arenas
                    .Any(a => a.CityId == cityId))
                .Distinct()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                })
                .ToListAsync();

        public async Task<string> GetImageByNameAsync(string sport)
            => await this.sportsRepository
                .All()
                .Where(s => s.Name == sport)
                .Select(s => s.Image)
                .FirstOrDefaultAsync();

        public async Task<string> GetNameByIdAsync(int? sportId)
            => await this.GetAsIQueryable(sportId.Value)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

        // Admin
        public async Task<IEnumerable<T>> GetAllAsync<T>(int? take = null, int skip = 0)
        {
            var query = this.sportsRepository.All()
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
            var sport = this.GetAsIQueryable(id);

            return await sport.To<T>().FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string name, string image)
        {
            if (await this.sportsRepository.All().AnyAsync(c => c.Name == name)
               || name == null)
            {
                throw new ArgumentException(string.Format(SportExistsMessage, name));
            }

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
            if (this.sportsRepository.All().Any(s => s.Name == name && s.Id == id))
            {
                throw new ArgumentException(string.Format(SportExistsMessage, name));
            }

            var sport = await this.GetAsIQueryable(id).FirstOrDefaultAsync();

            sport.Name = name;
            sport.Image = image;

            this.sportsRepository.Update(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var sport = await this.GetAsIQueryable(id).FirstOrDefaultAsync();
            this.sportsRepository.Delete(sport);
            await this.sportsRepository.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
            => await this.sportsRepository
                .All()
                .CountAsync();

        private IQueryable<Sport> GetAsIQueryable(int id)
            => this.sportsRepository
                .All()
                .Where(s => s.Id == id);
    }
}
