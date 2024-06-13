namespace InvoiceFetcher.Domains.Base
{
    public interface IBaseEntity { }
    public abstract class BaseEntity : IBaseEntity
    {
        public long Id { get; }

        public int VersionNumber { get; private set; }

        public long? DeletedBy { get; private set; }

        public long? UpdatedBy { get; private set; }

        public long CreatedBy { get; private set; }

        public DateTime? DeletedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public virtual void Updated(long? id)
        {
            UpdatedAt = DateTime.UtcNow;
            VersionNumber++;
            UpdatedBy = id;
        }

        public virtual void Deleted(long? id)
        {
            DeletedAt = DateTime.UtcNow;
            VersionNumber++;
            DeletedBy = id;
        }

        public virtual void Created(long? id)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = id ?? 0;
            VersionNumber = 1;
        }
    }
}
