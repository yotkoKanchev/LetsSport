namespace LetsSport.Web
{
    using System.Reflection;

    using LetsSport.Data;
    using LetsSport.Data.Seeding;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.Extensions;
    using LetsSport.Web.Hubs;
    using LetsSport.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                    .AddRazorPages();

            services
                    .AddDatabase(this.configuration)
                    .AddDataRepositories()
                    .AddIdentity()
                    .AddApplicationServices()
                    .AddApplicationControllers()
                    .AddSingleton(this.configuration)
                    .AddResponseCompression()
                    .AddApplicationInsightsTelemetry()
                    .AddTwoFactorAuthentication(this.configuration)
                    .ConfigureCookiePolicyOptions()
                    .SetClientLocation()
                    .AddCloudinary(this.configuration)
                    .AddEmailSender(this.configuration)
                    .AddSession()
                    .AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

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

            app.UseResponseCompression();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapHub<ChatHub>("/events/details");
                        endpoints.MapControllerRoute("paging", "{area:exists}/{controller}/{action}");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
