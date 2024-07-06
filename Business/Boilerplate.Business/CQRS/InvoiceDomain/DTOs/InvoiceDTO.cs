namespace Boilerplate.Business.CQRS.InvoiceDomain.DTOs
{
    public class InvoiceDTO
    {
        public long Id { get; set; }

        public string ExternalId { get; set; }

        public string CompanyIdentifier { get; set; }

        public DateTime ExternalCreatedAt { get; set; }
    }
}
