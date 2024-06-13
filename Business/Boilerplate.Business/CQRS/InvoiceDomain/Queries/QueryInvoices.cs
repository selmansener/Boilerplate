using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;

using InvoiceFetcher.Business.CQRS.InvoiceDomain.DTOs;
using InvoiceFetcher.Business.DTOs;
using InvoiceFetcher.DataAccess.Repositories;
using InvoiceFetcher.Domains.InvoiceDomain;

using Mapster;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InvoiceFetcher.Business.CQRS.InvoiceDomain.Queries
{
    public class QueryInvoices : IRequest<DQBResultDTO<InvoiceDTO>>
    {
        public QueryInvoices(DynamicQueryOptions dqbOptions)
        {
            DqbOptions = dqbOptions;
        }

        public DynamicQueryOptions DqbOptions { get; private set; }
    }

    internal class QueryInvoicesHandler : IRequestHandler<QueryInvoices, DQBResultDTO<InvoiceDTO>>
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;

        public QueryInvoicesHandler(IBaseRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<DQBResultDTO<InvoiceDTO>> Handle(QueryInvoices request, CancellationToken cancellationToken)
        {
            var data = await _invoiceRepository.GetAllAsNoTracking()
                .ProjectToType<InvoiceDTO>()
                .ApplyFilters(request.DqbOptions)
                .ToListAsync(cancellationToken);

            return new DQBResultDTO<InvoiceDTO>
            {
                Data = data,
                Count = request.DqbOptions.PaginationOption.DataSetCount
            };
        }
    }
}
