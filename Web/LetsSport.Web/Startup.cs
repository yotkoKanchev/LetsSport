namespace LetsSport.Web
{
    using System.Reflection;

    using CloudinaryDotNet;
    using LetsSport.Data;
    using LetsSport.Data.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Repositories;
    using LetsSport.Data.Seeding;
    using LetsSport.Services.Data;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.Infrastructure;
    using LetsSport.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
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
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(this.configuration["SendGrid:ApiKey"]));

            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IArenasService, ArenasService>();
            services.AddTransient<IEventsService, EventsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<ICitiesService, CitiesService>();
            services.AddTransient<IMessagesService, MessagesService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<ISportsService, SportsService>();
            services.AddTransient<IContactsService, ContactsService>();
            services.AddTransient<IReportsService, ReportsService>();
            services.AddTransient<IRentalRequestsService, RentalRequestsService>();

            // Scoped services
            services.AddScoped<ILocationLocator, LocationLocator>();

            // Singleton services
            Account account = new Account(
                this.configuration["Cloudinary:ApiName"],
                this.configuration["Cloudinary:ApiKey"],
                this.configuration["Cloudinary:ApiSecret"]);

            Cloudinary cloudinary = new Cloudinary(account);

            services.AddSingleton(cloudinary);

            // Sessions
            services.AddSession();

            // TwoFactorAuth
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = this.configuration["Facebook:AppId"];
                facebookOptions.AppSecret = this.configuration["Facebook:AppSecret"];
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = this.configuration["Google:ClientId"];
                googleOptions.ClientSecret = this.configuration["Google:ClientSecret"];
            });

            services.AddResponseCompression();
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
                app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
                app.UseDatabaseErrorPage();
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
                        endpoints.MapControllerRoute("paging", "{area:exists}/{controller}/{action}");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
