namespace LetsSport.Web.ViewModels.Arenas
{
    public class ArenaEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public double PricePerHour { get; set; }

        public string PhoneNumber { get; set; }

        public string WebUrl { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        //TODO add Country, City and Address props...
    }
}
