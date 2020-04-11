namespace LetsSport.Web.ViewModels.ArenaRequests
{
    public class RequestViewModel
    {
        public string Id { get; set; }

        public int EventId { get; set; }

        public EventInfoViewModel EventInfo { get; set; }
    }
}
