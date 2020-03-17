namespace LetsSport.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SportsService : ISportsService
    {
        private const string InvalidSportIdErrorMessage = "Sport with name: `{0}` does not exist.";

        private readonly IRepository<Sport> sportsRepository;

        public SportsService(IRepository<Sport> sportsRepository)
        {
            this.sportsRepository = sportsRepository;
        }

        public IEnumerable<SelectListItem> GetAll()
        {
            var sports = this.sportsRepository.All();

            var resultList = new List<SelectListItem>();

            foreach (var sport in sports)
            {
                resultList.Add(new SelectListItem { Value = sport.Id.ToString(), Text = sport.Name });
            }

            return resultList;
        }

        public IEnumerable<SelectListItem> GetAllSportsInCountry(string countryName)
        {
            var sports = this.sportsRepository.All()
                .Where(s => s.Arenas
                    .Any(a => a.Address.City.Country.Name == countryName));

            var resultList = new HashSet<SelectListItem>();

            foreach (var sport in sports)
            {
                resultList.Add(new SelectListItem { Value = sport.Id.ToString(), Text = sport.Name });
            }

            return resultList;
        }

        public int GetSportId(string sport)
        {
            var sporId = this.sportsRepository
               .All()
               .Where(s => s.Name == sport)
               .Select(s => s.Id)
               .FirstOrDefault();

            if (sporId == 0)
            {
                throw new ArgumentNullException(string.Format(InvalidSportIdErrorMessage, sport));
            }

            return sporId;
        }
    }
}
