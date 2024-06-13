using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InvoiceFetcher.Domains.Base;

namespace InvoiceFetcher.DataAccess.EntityConfigurations
{
    internal abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{typeof(T).Name}_Seq");
            builder.HasQueryFilter(x => x.DeletedAt.HasValue == false);
            builder.ToTable(tableBuilder => tableBuilder.IsTemporal());
        }
    }
}
