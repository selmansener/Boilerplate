using InvoiceFetcher.Domains.Base;
using InvoiceFetcher.Domains.DocumentDomain;
using InvoiceFetcher.Shared.Enums;

namespace InvoiceFetcher.Domains.InvoiceDomain
{
    public class Invoice : BaseEntity
    {
        private readonly List<Document> _relatedDocuments = new List<Document>();

        public Invoice(string externalId, string companyIdentifier, DateTime externalCreatedAt)
        {
            ExternalId = externalId;
            CompanyIdentifier = companyIdentifier;
            ExternalCreatedAt = externalCreatedAt;
        }

        public string ExternalId { get; private set; }

        public string CompanyIdentifier { get; private set; }

        public DateTime ExternalCreatedAt { get; private set; }

        public IReadOnlyList<Document> RelatedDocuments => _relatedDocuments;

        public void AddDocument(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (document.Type != DocumentType.Invoice)
            {
                throw new InvalidOperationException($"DocumentType must be {nameof(DocumentType.Invoice)}");
            }

            _relatedDocuments.Add(document);
        }

        public Document? FindDocument(Guid id)
        {
            return _relatedDocuments.FirstOrDefault(x => x.DocumentId == id);
        }

        public Document? FindDocument(string name)
        {
            return _relatedDocuments.FirstOrDefault(x => x.Name == name);
        }

        public void UpdateCompany(string companyIdentifier)
        {
            if (string.IsNullOrEmpty(companyIdentifier))
            {
                throw new ArgumentNullException(companyIdentifier);
            }

            if (ExternalCreatedAt.AddMonths(1) <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Updating company information of invoice is forbidden for older than 1 month records.");
            }

            CompanyIdentifier = companyIdentifier;
        }
    }
}
