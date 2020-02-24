namespace LetsSport.Web.ViewModels.Events
{
    using System.ComponentModel;

    public class EventInfoViewModel
    {
        public int Id { get; set; }

        public string Arena { get; set; }

        public string Sport { get; set; }

        public string Date { get; set; }

        [DisplayName("Empty spots")]
        public int EmptySpotsLeft { get; set; }

        public string ImgUrl { get; set; }
    }
}
