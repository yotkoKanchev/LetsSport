namespace LetsSport.Web.Extensions
{
    using LetsSport.Services.Data;
    using LetsSport.Services.Messaging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
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
    }
}
