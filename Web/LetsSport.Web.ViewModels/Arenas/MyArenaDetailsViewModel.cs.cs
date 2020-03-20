namespace LetsSport.Web.ViewModels.Arenas
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class MyArenaDetailsViewModel : IMapFrom<Arena>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportName { get; set; }

        [DisplayName("Price per Hour")]
        public double PricePerHour { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web-address")]
        public string WebUrl { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string AddressCityName { get; set; }

        public string AddressCityCountryName { get; set; }

        public string AddressStreetAddress { get; set; }

        [DisplayName("Administrator")]
        public string ArenaAdminUserName { get; set; }

        public string ArenaAdminId { get; set; }

        public string MainImageUrl { get; set; }

        public IEnumerable<string> Pictures { get; set; }

        public int EventsCount { get; set; }

        public string LoggedUserId { get; set; }

        public IFormFile NewMainImage { get; set; }
    }
}
