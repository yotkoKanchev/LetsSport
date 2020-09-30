namespace LetsSport.Web.Extensions
{
    using LetsSport.Data;
    using LetsSport.Data.Common;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Repositories;
    using LetsSport.Services.Data.Arenas;
    using LetsSport.Services.Data.Cities;
    using LetsSport.Services.Data.Cloudinary;
    using LetsSport.Services.Data.Common;
    using LetsSport.Services.Data.Contacts;
    using LetsSport.Services.Data.Countries;
    using LetsSport.Services.Data.Events;
    using LetsSport.Services.Data.Images;
    using LetsSport.Services.Data.Messages;
    using LetsSport.Services.Data.RentalRequests;
    using LetsSport.Services.Data.Reports;
    using LetsSport.Services.Data.Sports;
    using LetsSport.Services.Data.Users;
    using LetsSport.Services.Messaging;
    using LetsSport.Web.Filters;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(configuration.GetDefaultConnection()));

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddTransient<ISettingsService, SettingsService>()
                .AddTransient<IArenasService, ArenasService>()
                .AddTransient<IEventsService, EventsService>()
                .AddTransient<ICountriesService, CountriesService>()
                .AddTransient<ICitiesService, CitiesService>()
                .AddTransient<IMessagesService, MessagesService>()
                .AddTransient<IUsersService, UsersService>()
                .AddTransient<IImagesService, ImagesService>()
                .AddTransient<ISportsService, SportsService>()
                .AddTransient<IContactsService, ContactsService>()
                .AddTransient<IReportsService, ReportsService>()
                .AddTransient<IRentalRequestsService, RentalRequestsService>();

            return services;
        }

        public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(configuration["SendGrid:ApiKey"]));
            return services;
        }

        public static IServiceCollection AddApplicationControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews(configure =>
            {
                configure.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            return services;
        }

        public static IServiceCollection AddDataRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>))
                .AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
                .AddScoped<IDbQueryRunner, DbQueryRunner>();
            return services;
        }

        public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(x => CloudinaryFactory.GetInstance(configuration))
                    .AddSingleton<ICloudinaryHelper, CloudinaryHelper>()
                    .AddTransient<IApplicationCloudinary, ApplicationCloudinary>();

            return services;
        }

        public static IServiceCollection AddTwoFactorAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Facebook:AppSecret"];
            });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Google:ClientId"];
                googleOptions.ClientSecret = configuration["Google:ClientSecret"];
            });

            return services;
        }

        public static IServiceCollection SetClientLocation(this IServiceCollection services)
        {
            services.AddScoped<ILocationLocator, LocationLocator>()
                    .AddScoped<SetLocationResourceFilter>();

            return services;
        }

        public static IServiceCollection ConfigureCookiePolicyOptions(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            return services;
        }
    }
}
