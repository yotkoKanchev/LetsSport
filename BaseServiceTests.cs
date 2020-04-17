public abstract class BaseServiceTests : IDisposable
{
    protected BaseServiceTests()
    {
        var services = this.SetServices();

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

        services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
        });

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
        services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
        services.AddTransient(typeof(ILoggerFactory), typeof(LoggerFactory));
        services.AddTransient<IEmailSender>(x => new SendGridEmailSender(new LoggerFactory(), "SendGridKey"));
        services.AddTransient<IQuizzesService, QuizzesService>();
        services.AddTransient<IQuestionsService, QuestionsService>();
        services.AddTransient<IAnswersService, AnswersService>();
        services.AddTransient<IEventsGroupsService, EventsGroupsService>();
        services.AddTransient<IGroupsService, GroupsService>();
        services.AddTransient<IResultsService, ResultsService>();
        services.AddTransient<IEventsService, EventsService>();
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IStudentsGroupsService, StudentsGroupsService>();
        services.AddTransient<ICategoriesService, CategoriesService>();
        services.AddTransient<IScheduledJobsService, ScheduledJobsService>();

        // AutoMapper
        AutoMapperConfig.RegisterMappings(typeof(EventListViewModel).GetTypeInfo().Assembly);

        // SignalR Setup
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        var context = new DefaultHttpContext();
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

        return services;
    }
}
