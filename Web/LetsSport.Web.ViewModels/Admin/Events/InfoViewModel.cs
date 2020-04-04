namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;

    public class InfoViewModel : IMapFrom<Event>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public int CityId { get; set; }

        public string SportName { get; set; }

        public int SportId { get; set; }

        public string ArenaName { get; set; }

        public int ArenaId { get; set; }

        public EventStatus Status { get; set; }

        public DateTime Date { get; set; }
    }
}
