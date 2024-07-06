using Boilerplate.DataAccess.Development;
using Boilerplate.DataAccess.Repositories;
using Boilerplate.DataAccess.Transactions;
using Boilerplate.Infrastructure.EventSourcing;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Boilerplate.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            if (!services.Any(x => x.ServiceType == typeof(IHttpContextAccessor)))
            {
                services.AddHttpContextAccessor();
            }

            // TODO: move this to api level
            services.AddEventSourcing(new EventSourcingOptions
            {
                Username = "elastic",
                Password = "qwe123**",
                ElasticHostUrl = "https://localhost:9200",
                CertificateFingerprint = "969A4D81BE12F58F7F0E3CA2765FEC246DD8F00280AFAF0AD6C5D85544242A57"
            });

            services.AddPooledDbContextFactory<BoilerplateDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                });
                options.AddInterceptors(new EventTrackingSaveChangesInterceptor());
            });
            services.AddScoped<BoilerplateDbContextFactory>();
            services.AddScoped(
                sp => sp.GetRequiredService<BoilerplateDbContextFactory>().CreateDbContext());

            services.AddScoped<IMigrationContext>(sp =>
            {
                var environment = sp.GetRequiredService<IHostEnvironment>();

                if (!environment.IsDevelopment())
                {
                    return null;
                }

                var dbContext = sp.GetRequiredService<BoilerplateDbContext>();

                return new MigrationContext(dbContext);
            });

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<ITransactionManager, TransactionManager>();

            return services;
        }
    }
}
