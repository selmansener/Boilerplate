using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InvoiceFetcher.Infrastructure.EventSourcing;
using InvoiceFetcher.Shared.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace InvoiceFetcher.DataAccess
{
    internal class InvoiceFetcherDbContextFactory : IDbContextFactory<InvoiceFetcherDbContext>
    {
        private const int DefaultTenantId = -1;

        private readonly IDbContextFactory<InvoiceFetcherDbContext> _pooledFactory;
        private readonly int _tenantId;
        private readonly EventCollection _eventCollection;
        private readonly IEventSourceContext _eventSourceContext;

        public InvoiceFetcherDbContextFactory(
            IDbContextFactory<InvoiceFetcherDbContext> pooledFactory,
            ITenant tenant,
            EventCollection eventCollection,
            IEventSourceContext eventSourceContext)
        {
            _pooledFactory = pooledFactory;
            _tenantId = tenant?.TenantId ?? DefaultTenantId;
            _eventCollection = eventCollection;
            _eventSourceContext = eventSourceContext;
        }

        public InvoiceFetcherDbContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            context.TenantId = _tenantId;
            context.EventCollection = _eventCollection;
            context.EventSourceContext = _eventSourceContext;
            return context;
        }
    }
}
