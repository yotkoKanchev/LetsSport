namespace LetsSport.Web.Extensions
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        public static string GetDefaultConnection(this IConfiguration configuration)
            => configuration.GetConnectionString("DefaultConnection");
    }
}
