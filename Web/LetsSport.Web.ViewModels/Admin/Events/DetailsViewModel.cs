namespace LetsSport.Web.ViewModels.Admin.Events
{
    using System;

    using AutoMapper;
    using LetsSport.Data.Models.Events;
    using LetsSport.Data.Models.Users;
    using LetsSport.Services.Mapping;

    public class DetailsViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportName { get; set; }

        public string CountryName { get; set; }

        public int CountryId { get; set; }

        public string CityName { get; set; }

        public string AdminName { get; set; }

        public string ArenaName { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingHour { get; set; }

        public double ArenaPricePerHour { get; set; }

        public double DurationInHours { get; set; }

        public string GameFormat { get; set; }

        public int MaxPlayers { get; set; }

        public int MinPlayers { get; set; }

        public int UsersCount { get; set; }

        public Gender Gender { get; set; }

        public EventStatus Status { get; set; }

        public string AdditionalInfo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, DetailsViewModel>()
                .ForMember(a => a.AdminName, opt => opt.MapFrom(a => a.Admin.FirstName + " " + a.Admin.LastName));
        }
    }
}
