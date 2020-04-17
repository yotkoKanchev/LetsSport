namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.Reflection;

    using AutoMapper;
    using CloudinaryDotNet;
    using LetsSport.Data;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Repositories;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.Common;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using LetsSport.Web.ViewModels.Admin.Countries;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public abstract class BaseServiceTests : IDisposable
    {
        protected BaseServiceTests()
        {
            var services = this.SetServices();
            this.RegisterMappings();

            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        public void Dispose()
        {
            this.DbContext.Database.EnsureDeleted();
            this.SetServices();
        }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services
                 .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                 {
                     options.Password.RequireDigit = false;
                     options.Password.RequireLowercase = false;
                     options.Password.RequireUppercase = false;
                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequiredLength = 6;
                 })
                 .AddEntityFrameworkStores<ApplicationDbContext>();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Application services
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IEmailSender, SendGridEmailSender>(provider =>
                new SendGridEmailSender("SendGridKey"));
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender("SendGridKey"));
            var cloudinaryAccount = new CloudinaryDotNet.Account(CloudinaryConfig.ApiName, CloudinaryConfig.ApiKey, CloudinaryConfig.ApiSecret);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);


            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IContactsService, ContactsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<ICitiesService, CitiesService>();
            services.AddTransient<ISportsService, SportsService>();
            services.AddTransient<IMessagesService, MessagesService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<IRentalRequestsService, RentalRequestsService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IArenasService, ArenasService>();
            services.AddTransient<IEventsService, EventsService>();
            services.AddTransient<IReportsService, ReportsService>();

            //Account account = new Account("ApiName", "ApiKey", "ApiSecret");
            //Cloudinary cloudinary = new Cloudinary(account);
            //services.AddSingleton(cloudinary);

            // SignalR Setup
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            return services;
        }

        private void RegisterMappings()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ContactIndexViewModel).GetTypeInfo().Assembly,
                typeof(CountryInfoViewModel).GetTypeInfo().Assembly,
                typeof(CityInfoViewModel).GetTypeInfo().Assembly);
        }
    }
}
