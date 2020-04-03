﻿namespace LetsSport.Web.ViewModels.Admin.Cities
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class DeleteViewModel : IMapFrom<City>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
