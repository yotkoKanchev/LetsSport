namespace LetsSport.Web.Extensions
{
    using LetsSport.Common;
    using LetsSport.Data;
    using LetsSport.Data.Seeding;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var dbContext = services.ServiceProvider.GetService<ApplicationDbContext>();

            dbContext.Database.Migrate();

            return app;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();

            return app;
        }

        public static IApplicationBuilder SetExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
                // app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            return app;
        }

        public static IApplicationBuilder SetLocation(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                using var serviceScope = app.ApplicationServices.CreateScope();
                var locationLocator = serviceScope.ServiceProvider.GetRequiredService<ILocationLocator>();

                if (context.Session.GetString(GlobalConstants.Country) == null
                   || context.Session.GetString(GlobalConstants.City) == null)
                {
                    var ip = context.Connection.RemoteIpAddress.ToString();

                    var (country, city) = locationLocator.GetLocationInfo(ip);

                    context.Session.SetString(GlobalConstants.City, city);
                    context.Session.SetString(GlobalConstants.Country, country);
                }

                await next();
            });

            return app;
        }
    }
}
