using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Boilerplate.DataAccess.Development
{
    public interface IMigrationContext
    {
        Task EnsureCreated();

        Task Migrate();

        Task<IEnumerable<string>> GetPendingMigrations();
    }

    internal class MigrationContext : IMigrationContext
    {
        private readonly BoilerplateDbContext _context;

        public MigrationContext(BoilerplateDbContext context)
        {
            _context = context;
        }

        public async Task EnsureCreated()
        {
            var created = await _context.Database.EnsureCreatedAsync();

            if (!created)
            {
                throw new InvalidOperationException("Database must be empty or not exists to create.");
            }
        }

        public async Task Migrate()
        {
            await _context.Database.MigrateAsync();
        }

        public async Task<IEnumerable<string>> GetPendingMigrations()
        {
            return await _context.Database.GetPendingMigrationsAsync();
        }
    }
}
