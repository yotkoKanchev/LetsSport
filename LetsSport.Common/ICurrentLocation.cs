namespace LetsSport.Common
{
    public interface ILocationLocator
    {
        public (string Country, string City) GetLocationInfo();
    }
}
