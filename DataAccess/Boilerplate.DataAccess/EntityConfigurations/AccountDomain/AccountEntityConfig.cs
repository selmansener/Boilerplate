using InvoiceFetcher.Domains.AccountDomain;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceFetcher.DataAccess.EntityConfigurations.AccountDomain
{
    internal class AccountEntityConfig : BaseEntityConfiguration<Account>
    {
        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            base.Configure(builder);
        }
    }
}
