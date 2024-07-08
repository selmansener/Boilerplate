using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Boilerplate.Infrastructure.EventSourcing;
using Boilerplate.Shared.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Boilerplate.DataAccess
{
    internal class BoilerplateDbContextFactory : IDbContextFactory<BoilerplateDbContext>
    {
        private const int DefaultTenantId = -1;

        private readonly IDbContextFactory<BoilerplateDbContext> _pooledFactory;
        private readonly int _tenantId;
        private readonly EventCollection _eventCollection;
        private readonly IEventSourceContext _eventSourceContext;

        public BoilerplateDbContextFactory(
            IDbContextFactory<BoilerplateDbContext> pooledFactory,
            ITenant tenant,
            EventCollection eventCollection,
            IEventSourceContext eventSourceContext)
        {
            _pooledFactory = pooledFactory;
            _tenantId = tenant?.TenantId ?? DefaultTenantId;
            _eventCollection = eventCollection;
            _eventSourceContext = eventSourceContext;
        }

        public BoilerplateDbContext CreateDbContext()
        {
            var context = _pooledFactory.CreateDbContext();
            context.TenantId = _tenantId;
            context.EventCollection = _eventCollection;
            context.EventSourceContext = _eventSourceContext;
            return context;
        }
    }
}
