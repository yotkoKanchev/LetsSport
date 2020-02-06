namespace LetsSport.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models;
    using LetsSport.Data.Models.AddressModels;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.ChatModels;
    using LetsSport.Data.Models.EventModels;
    using LetsSport.Data.Models.Mappings;
    using LetsSport.Data.Models.SporterModels;
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

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Neighborhood> Neighborhoods { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Arena> Arenas { get; set; }

        public DbSet<ArenaAdmin> ArenaAdmins { get; set; }

        public DbSet<ArenaRentalRequest> ArenaRentalRequests { get; set; }

        public DbSet<ChatRoom> ChatRooms { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<ChatRoomSporter> ChatRoomsSporters { get; set; }

        public DbSet<EventSporter> EventsSporters { get; set; }

        public DbSet<Sporter> Sporters { get; set; }

        public DbSet<Setting> Settings { get; set; }

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

            builder.Entity<ChatRoomSporter>()
                .HasKey(e => new
                {
                    e.ChatRoomId,
                    e.SporterId,
                });

            builder.Entity<EventSporter>()
                .HasKey(e => new
                {
                    e.EventId,
                    e.SporterId,
                });

            builder.Entity<Event>()
                .HasOne(e => e.ChatRoom)
                .WithOne(ch => ch.Event)
                .HasForeignKey<ChatRoom>(r => r.EventId);

            builder.Entity<Event>()
               .HasOne(e => e.Admin)
               .WithMany(s => s.AdministratingEvents)
               .HasForeignKey(e => e.AdminId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
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
