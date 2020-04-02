namespace LetsSport.Web.ViewModels.Admin.Arenas
{
    using System;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;

    public class DeleteViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string CityName { get; set; }

        public string Address { get; set; }

        public string SportName { get; set; }

        public double PricePerHour { get; set; }

        public string PhoneNumber { get; set; }

        public string WebUrl { get; set; }

        public string Email { get; set; }

        public ArenaStatus Status { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
