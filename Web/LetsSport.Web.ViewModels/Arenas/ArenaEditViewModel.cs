namespace LetsSport.Web.ViewModels.Arenas
{
    using System.ComponentModel;

    public class ArenaEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Sport Type")]
        public string SportType { get; set; }

        [DisplayName("Price per Hour")]

        public double PricePerHour { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Web address")]
        public string WebUrl { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        [DisplayName("Street Address")]
        public string StreetAddress { get; set; }
    }
}
