namespace LetsSport.Web.Infrastructure
{
    public interface ILocationLocator
    {
        public (string Country, string City) GetLocationInfo(string ip);
    }
}
