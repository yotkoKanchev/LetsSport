namespace LetsSport.Services.Data.Common
{
    using System.Collections.Generic;
    using System.Linq;

    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class SettingsService : ISettingsService
    {
        private readonly IDeletableEntityRepository<Setting> settingsRepository;

        public SettingsService(IDeletableEntityRepository<Setting> settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        public int GetCount()
            => this.settingsRepository.All().Count();

        public IEnumerable<T> GetAll<T>()
            => this.settingsRepository.All().To<T>().ToList();
    }
}
