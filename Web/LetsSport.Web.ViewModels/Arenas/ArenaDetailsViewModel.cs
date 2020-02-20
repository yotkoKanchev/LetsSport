namespace LetsSport.Web.ViewModels.Arenas
{
    public class ArenaDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Sport { get; set; }

        public double PricePerHour { get; set; }

        public string PhoneNumber { get; set; }

        public string WebUrl { get; set; }

        //public double Rating { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string ArenaAdmin { get; set; }
    }
}
