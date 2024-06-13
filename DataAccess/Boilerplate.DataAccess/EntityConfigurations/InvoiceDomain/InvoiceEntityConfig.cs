using InvoiceFetcher.Domains.InvoiceDomain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFetcher.DataAccess.EntityConfigurations.InvoiceDomain
{
    internal class InvoiceEntityConfig : BaseEntityConfiguration<Invoice>
    {
        public override void Configure(EntityTypeBuilder<Invoice> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => new { x.ExternalId, x.CompanyIdentifier, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);
        }
    }
}
