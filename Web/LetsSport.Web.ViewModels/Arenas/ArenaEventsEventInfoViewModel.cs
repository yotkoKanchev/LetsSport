﻿namespace LetsSport.Web.ViewModels.Arenas
{
    using System;

    using AutoMapper;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Services.Mapping;

    public class ArenaEventsEventInfoViewModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartingHour { get; set; }

        public DateTime EndingHour { get; set; }

        public /*ArenaRentalRequestStatus*/string ArenaRequestStatus { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, ArenaEventsEventInfoViewModel>()
                .ForMember(e => e.EndingHour, opt => opt.MapFrom(e => e.StartingHour.AddHours(e.DurationInHours)))
                .ForMember(e => e.ArenaRequestStatus, opt => opt.MapFrom(e => "To be mapped"));
        }
    }
}