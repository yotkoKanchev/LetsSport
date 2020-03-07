namespace LetsSport.Services.Data.Common
{
    public interface ILocationLocator
    {
        public (string Country, string City) GetLocationInfo();
    }
}
