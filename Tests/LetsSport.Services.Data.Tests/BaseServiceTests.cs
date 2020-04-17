namespace LetsSport.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using AutoMapper;
    using CloudinaryDotNet;
    using LetsSport.Data;
    using LetsSport.Data.Common.Repositories;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.UserModels;
    using LetsSport.Data.Repositories;
    using LetsSport.Services.Data;
    using LetsSport.Services.Data.Cloudinary;
    using LetsSport.Services.Mapping;
    using LetsSport.Services.Messaging;
    using LetsSport.Web;
    using LetsSport.Web.ViewModels.Admin.Cities;
    using LetsSport.Web.ViewModels.Admin.Countries;
    using LetsSport.Web.ViewModels.Contacts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public abstract class BaseServiceTests : IDisposable
    {
        protected BaseServiceTests()
        {
            this.Configuration = this.SetConfiguration();
            var services = this.SetServices();
            this.RegisterMappings();
            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            this.Seed();
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        protected IConfigurationRoot Configuration { get; set; }

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
            services.AddSingleton<IConfiguration>(this.Configuration);

            var cloudinaryAccount = new CloudinaryDotNet.Account(
                this.Configuration["Cloudinary:ApiName"],
                this.Configuration["Cloudinary:ApiKey"],
                this.Configuration["Cloudinary:ApiSecret"]);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);
            services.AddSingleton<ICloudinaryHelper, CloudinaryHelper>();

            services.AddTransient<IApplicationCloudinary, ApplicationCloudinary>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IContactsService, ContactsService>();
            services.AddTransient<IReportsService, ReportsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<ICitiesService, CitiesService>();
            services.AddTransient<ISportsService, SportsService>();
            services.AddTransient<IMessagesService, MessagesService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddTransient<IRentalRequestsService, RentalRequestsService>();
            services.AddTransient<IUsersService, UsersService>();

            services.AddTransient<IArenasService, ArenasService>();
            services.AddTransient<IEventsService, EventsService>();

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

        private IConfigurationRoot SetConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                 path: "appsettings.json",
                 optional: false,
                 reloadOnChange: true)
           .Build();
        }

        private void Seed()
        {
            var country = new Country
            {
                Name = "testCountry",
            };

            this.DbContext.Countries.Add(country);

            var city = new City
            {
                CountryId = 1,
                Name = "testCity",
                IsDeleted = false,
            };

            this.DbContext.Cities.Add(city);

            var sport = new Sport
            {
                Name = "testSport",
                Image = "https://sportUrl",
            };

            this.DbContext.Sports.Add(sport);

            var user = new ApplicationUser
            {
                Email = "user@abv.bg",
                PasswordHash = "passsword",
                CityId = 1,
                CountryId = 1,
                UserName = "tester",
            };

            this.DbContext.ApplicationUsers.Add(user);
            this.DbContext.SaveChanges();

            var userId = this.DbContext.ApplicationUsers.Select(u => u.Id).First();
            var arena = new Arena
            {
                Name = "Arena",
                SportId = 1,
                ArenaAdminId = userId,
                CityId = 1,
                CountryId = 1,
                PricePerHour = 20,
                PhoneNumber = "0888899898",
                Status = ArenaStatus.Active,
            };

            this.DbContext.Arenas.Add(arena);
            this.DbContext.SaveChanges();

            var evt = new Event
            {
                CountryId = 1,
                CityId = 1,
                Name = "Event",
                Date = new DateTime(2020, 05, 05),
                StartingHour = new DateTime(2020, 05, 05, 12, 00, 00),
                Status = EventStatus.AcceptingPlayers,
                RequestStatus = ArenaRequestStatus.NotSent,
                MinPlayers = 0,
                MaxPlayers = 1,
                Gender = Gender.Any,
                DurationInHours = 1,
                SportId = 1,
                AdminId = userId,
                ArenaId = this.DbContext.Arenas.Select(a => a.Id).First(),
            };

            this.DbContext.Events.Add(evt);
            this.DbContext.SaveChanges();

            var message = new Message
            {
                SenderId = userId,
                EventId = 1,
                Content = "testMessage",
            };

            this.DbContext.Messages.Add(message);
            this.DbContext.SaveChanges();

            var recipient = new ApplicationUser
            {
                Email = "recipient@abv.bg",
                PasswordHash = "passsword",
                CityId = 1,
                CountryId = 1,
            };

            this.DbContext.ApplicationUsers.Add(recipient);
            this.DbContext.SaveChanges();

            var recipientId = this.DbContext.ApplicationUsers.Select(u => u.Id).Skip(1).First();

            var report = new Report
            {
                Abuse = AbuseType.SexualHarisement,
                Content = "content",
                SenderId = userId,
                ReportedUserId = recipientId,
            };

            this.DbContext.Reports.Add(report);
            this.DbContext.SaveChanges();

            var eventId = this.DbContext.Events.Select(e => e.Id).First();

            var eventUser = new EventUser
            {
                UserId = userId,
                EventId = eventId,
            };

            this.DbContext.EventsUsers.Add(eventUser);
            this.DbContext.SaveChanges();
        }
    }
}
