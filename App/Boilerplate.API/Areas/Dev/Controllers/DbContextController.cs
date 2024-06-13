using InvoiceFetcher.DataAccess.Development;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceFetcher.API.Areas.Dev.Controllers
{
    public class DbContextController : DevAPIBaseController
    {
        private readonly IMigrationContext _migrationContext;

        public DbContextController(IMigrationContext migrationContext)
        {
            _migrationContext = migrationContext;
        }

        [HttpPost("EnsureCreated")]
        public async Task<IActionResult> EnsureCreated()
        {
            await _migrationContext.EnsureCreated();

            return Ok();
        }

        [HttpPost("Migrate")]
        public async Task<IActionResult> Migrate()
        {
            await _migrationContext.Migrate();

            return Ok();
        }

        [HttpGet("GetPendingMigrations")]
        public async Task<IActionResult> GetPendingMigrations()
        {
            var pendingMigrations = await _migrationContext.GetPendingMigrations();

            return Ok(pendingMigrations);
        }
    }
}
