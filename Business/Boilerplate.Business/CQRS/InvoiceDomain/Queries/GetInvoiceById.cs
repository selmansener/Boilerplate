using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

using Boilerplate.Business.CQRS.InvoiceDomain.DTOs;
using Boilerplate.DataAccess.Repositories;
using Boilerplate.Domains.InvoiceDomain;
using Boilerplate.Shared.Exceptions;

using Mapster;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Business.CQRS.InvoiceDomain.Queries
{
    public class GetInvoiceById : IRequest<InvoiceDTO>
    {
        public GetInvoiceById(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }

    internal class GetInvoiceByIdValidator : AbstractValidator<GetInvoiceById>
    {
        public GetInvoiceByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceById, InvoiceDTO>
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;

        public GetInvoiceByIdHandler(IBaseRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceDTO> Handle(GetInvoiceById request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetAllAsNoTracking().ProjectToType<InvoiceDTO>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (invoice == null)
            {
                throw new NotFoundException(nameof(Invoice), request.Id.ToString());
            }

            return invoice;
        }
    }
}
