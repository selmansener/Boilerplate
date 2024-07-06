using FluentValidation;

using Boilerplate.DataAccess.Repositories;
using Boilerplate.Domains.InvoiceDomain;
using Boilerplate.Shared.Exceptions;

using MediatR;

using Newtonsoft.Json;

namespace Boilerplate.Business.CQRS.InvoiceDomain.Commands
{
    public class UpdateInvoiceCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int InvoiceId { get; set; }

        public string CompanyIdentifier { get; set; }
    }

    internal class UpdateInvoiceCommandValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        public UpdateInvoiceCommandValidator()
        {
            RuleFor(x => x.CompanyIdentifier).NotEmpty();
        }
    }

    internal class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, Unit>
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;

        public UpdateInvoiceCommandHandler(IBaseRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Unit> Handle(UpdateInvoiceCommand command, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);

            if (invoice == null)
            {
                throw new NotFoundException(nameof(Invoice), nameof(Invoice.Id));
            }

            invoice.UpdateCompany(command.CompanyIdentifier);

            await _invoiceRepository.UpdateAsync(invoice, cancellationToken, saveChanges: true);

            return Unit.Value;
        }
    }
}
