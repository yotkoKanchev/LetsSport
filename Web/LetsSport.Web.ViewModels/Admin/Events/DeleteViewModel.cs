namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System;

    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;

    public class DeleteViewModel : IMapFrom<Event>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public string ArenaName { get; set; }

        public string SportName { get; set; }

        public double ArenaPricePerHour { get; set; }

        public EventStatus Status { get; set; }

        public string AdditionalInfo { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
