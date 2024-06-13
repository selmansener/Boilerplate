using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using InvoiceFetcher.Domains.Base;
using Microsoft.EntityFrameworkCore.Metadata;
using InvoiceFetcher.Domains.InvoiceDomain;
using InvoiceFetcher.Domains.AccountDomain;
using InvoiceFetcher.Domains.DocumentDomain;
using InvoiceFetcher.Infrastructure.EventSourcing;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace InvoiceFetcher.DataAccess
{
    internal class InvoiceFetcherDbContext : DbContext
    {
        private readonly IEnumerable<EntityState> _trackedStates = new List<EntityState>
        {
            EntityState.Modified,
            EntityState.Deleted,
            EntityState.Added
        };

        private readonly IDictionary<EntityState, Action<EntityEntry>> _stateActionMapping
            = new Dictionary<EntityState, Action<EntityEntry>>()
        {
            { EntityState.Modified, Updated },
            { EntityState.Added, Added  },
            { EntityState.Deleted, Deleted  }
        };

        public InvoiceFetcherDbContext(DbContextOptions<InvoiceFetcherDbContext> options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public int TenantId { get; set; }

        internal EventCollection EventCollection { get; set; }

        internal IEventSourceContext EventSourceContext { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Document> Documents { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties(typeof(decimal)).HavePrecision(28, 5);
            configurationBuilder.Properties(typeof(string)).HaveMaxLength(1000);

            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            var entityTypes = GetEntityTypes(modelBuilder).ToList();

            AddDeletedAtIsNullFilterToNonClusteredIndexes(modelBuilder, entityTypes);

            AddFillFactorToIndexes(modelBuilder, entityTypes);

            base.OnModelCreating(modelBuilder);
        }

        public new int SaveChanges()
        {
            ExecuteCommonActions();

            return base.SaveChanges();
        }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ExecuteCommonActions();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static void Deleted(EntityEntry entry)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.Deleted(0);
            entry.State = EntityState.Modified;
        }

        private static void Added(EntityEntry entry)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.Created(0);
        }

        private static void Updated(EntityEntry entry)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.Updated(0);
        }

        private void AddFillFactorToIndexes(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var entityTypeBuilder = builder.Entity(entityClrType);
                var indexes = entityTypeBuilder.Metadata.GetIndexes();
                foreach (var index in indexes)
                {
                    index.SetFillFactor(90);
                }
            }
        }

        private void AddDeletedAtIsNullFilterToNonClusteredIndexes(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            string deletedAtIndexFilter = $"[{nameof(BaseEntity.DeletedAt)}] IS NULL";
            string deletedAtNotNullIndexFilter = $"[{nameof(BaseEntity.DeletedAt)}] IS NOT NULL";
            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var entityTypeBuilder = builder.Entity(entityClrType);
                var nonClusteredIndexesWithDeletedAt = entityTypeBuilder.Metadata.GetIndexes().Where(i =>
                    (
                        !i.IsClustered().HasValue ||
                            (i.IsClustered().HasValue && !i.IsClustered().Value)
                    )
                    && i.Properties.Any(p => p.Name == nameof(BaseEntity.DeletedAt)));
                foreach (var index in nonClusteredIndexesWithDeletedAt)
                {
                    var indexFilter = index.GetFilter();
                    if (!string.IsNullOrEmpty(indexFilter) && indexFilter.Contains(deletedAtNotNullIndexFilter))
                    {
                        indexFilter = indexFilter.Replace(deletedAtNotNullIndexFilter, deletedAtIndexFilter);
                    }
                    else
                    {
                        indexFilter = deletedAtIndexFilter;
                    }
                    index.SetFilter(indexFilter);
                }
            }
        }

        private IEnumerable<IMutableEntityType> GetEntityTypes(ModelBuilder builder)
        {
            return builder.Model.GetEntityTypes().Where(x => typeof(IBaseEntity).IsAssignableFrom(x.ClrType) && x.ClrType != typeof(BaseEntity));
        }

        private void ExecuteCommonActions()
        {
            var entries = ChangeTracker.Entries().Where(e => _trackedStates.Contains(e.State));
            foreach (var entry in entries)
            {
                _stateActionMapping[entry.State](entry);

                var eventSource = entry.Metadata.DisplayName();
                var eventName = $"{eventSource}_{entry.State}";

                var @event = new EventBase(eventSource, eventName);

                foreach (var propertyEntry in entry.Properties)
                {
                    @event.AddField(propertyEntry.Metadata.Name, propertyEntry.CurrentValue);
                }

                EventCollection.Enqueue(@event);
            }
        }
    }

    internal class EventTrackingSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (eventData.Context is InvoiceFetcherDbContext dbContext)
            {
                dbContext.EventSourceContext.InsertEvents(dbContext.EventCollection);
            }

            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is InvoiceFetcherDbContext dbContext)
            {
                dbContext.EventSourceContext.InsertEvents(dbContext.EventCollection);
            }

            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }
    }
}
