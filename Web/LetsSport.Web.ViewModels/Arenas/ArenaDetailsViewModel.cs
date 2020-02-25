namespace LetsSport.Web.ViewModels.Arenas
{
    using System.ComponentModel;

    public class ArenaDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Sport { get; set; }

        [DisplayName("Price per Hour")]
        public string PricePerHour { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web-address")]
        public string WebUrl { get; set; }

        public string Rating { get; set; }

        public string Address { get; set; }

        [DisplayName("Administrator")]
        public string ArenaAdmin { get; set; }

        public string PhotoPath { get; set; }
    }
}
