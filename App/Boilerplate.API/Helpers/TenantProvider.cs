using InvoiceFetcher.Shared.Interfaces;

using Microsoft.Extensions.Primitives;

namespace InvoiceFetcher.API.Helpers
{
    public class TenantProvider : ITenant
    {
        public TenantProvider(IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor.HttpContext != null && contextAccessor.HttpContext.Request.Headers.TryGetValue("TenantId", out StringValues tenantId))
            {
                if (int.TryParse(tenantId, out int tenantIdValue))
                {
                    TenantId = tenantIdValue;
                }
            }

            TenantId = -1;
        }

        public int TenantId { get; set; }
    }
}
