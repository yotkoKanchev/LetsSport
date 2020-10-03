namespace LetsSport.Web
{
    using System.Reflection;

    using LetsSport.Services.Mapping;
    using LetsSport.Web.Extensions;
    using LetsSport.Web.Hubs;
    using LetsSport.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
                .AddSingleton(this.configuration)
                .AddDatabase(this.configuration)
                .AddDataRepositories()
                .AddIdentity()
                .AddApplicationServices()
                .AddApplicationControllers()
                .AddResponseCompression()
                .AddTwoFactorAuthentication(this.configuration)
                .ConfigureCookiePolicyOptions()
                .SetClientLocation()
                .AddCloudinary(this.configuration)
                .AddEmailSender(this.configuration)
                .AddSession()
                .AddApplicationInsightsTelemetry()
                .AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            app
                .ApplyMigrations()
                .SeedData()
                .SetExceptionHandling(env)
                .UseResponseCompression()
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseSession()
                .UseEndpoints(
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
