using InvoiceFetcher.Domains.Base;
using InvoiceFetcher.Shared.Enums;

namespace InvoiceFetcher.Domains.DocumentDomain
{
    public class Document : BaseEntity
    {
        public Document(string originalName, string url, DocumentType type)
        {
            Type = type;
            OriginalName = originalName;
            Url = url;
            DocumentId = Guid.NewGuid();
            Extension = originalName.Split('.').LastOrDefault();
            Name = $"{DocumentId}.{Extension}";
        }

        public string Url { get; private set; }

        public string OriginalName { get; private set; }

        public Guid DocumentId { get; private set; }

        public string Name { get; private set; }

        public string Extension { get; private set; }

        public DocumentType Type { get; private set; }
    }
}
