namespace LetsSport.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Arena> Arenas { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<ArenaRentalRequest> ArenaRentalRequests { get; set; }

        public DbSet<EventUser> EventsUsers { get; set; }

        public DbSet<Sport> Sports { get; set; }

        public DbSet<ContactForm> ContactForms { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Report> Reports { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureUserIdentityRelations(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventUser>()
                .HasKey(e => new
                {
                    e.EventId,
                    e.UserId,
                });

            builder.Entity<Event>()
                .HasOne(e => e.ArenaRentalRequest)
                .WithOne(arr => arr.Event)
                .HasForeignKey<ArenaRentalRequest>(ar => ar.EventId);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.AdministratingArena)
                .WithOne(au => au.ArenaAdmin)
                .HasForeignKey<Arena>(ar => ar.ArenaAdminId);

            builder.Entity<Image>()
                .HasOne(i => i.User)
                .WithOne(up => up.Avatar)
                .HasForeignKey<ApplicationUser>(up => up.AvatarId);

            builder.Entity<Image>()
               .HasOne(i => i.Arena)
               .WithOne(a => a.MainImage)
               .HasForeignKey<Arena>(a => a.MainImageId);

            builder.Entity<ApplicationUser>()
              .HasOne(i => i.AdministratingArena)
              .WithOne(a => a.ArenaAdmin)
              .HasForeignKey<Arena>(a => a.ArenaAdminId);

            builder.Entity<Event>()
               .HasOne(e => e.Admin)
               .WithMany(s => s.AdministratingEvents)
               .HasForeignKey(e => e.AdminId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
              .HasOne(e => e.City)
              .WithMany(s => s.Events)
              .HasForeignKey(e => e.CityId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
              .HasOne(e => e.Country)
              .WithMany(s => s.Events)
              .HasForeignKey(e => e.CountryId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Event>()
              .HasOne(e => e.Sport)
              .WithMany(s => s.Events)
              .HasForeignKey(e => e.SportId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Arena>()
               .HasOne(e => e.Country)
               .WithMany(s => s.Arenas)
               .HasForeignKey(e => e.CountryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Arena>()
              .HasOne(e => e.City)
              .WithMany(s => s.Arenas)
              .HasForeignKey(e => e.CityId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ArenaRentalRequest>()
              .HasOne(e => e.Arena)
              .WithMany(s => s.Requests)
              .HasForeignKey(e => e.ArenaId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Arena>()
              .HasOne(e => e.Sport)
              .WithMany(s => s.Arenas)
              .HasForeignKey(e => e.SportId)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasIndex(s => s.Email)
                .IsUnique();
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
