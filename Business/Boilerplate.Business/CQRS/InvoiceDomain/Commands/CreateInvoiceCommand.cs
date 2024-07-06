using System.Text.Json.Serialization;

using FluentValidation;

using Boilerplate.Business.Events;
using Boilerplate.DataAccess.Repositories;
using Boilerplate.Domains.InvoiceDomain;
using Boilerplate.Infrastructure.EventBus.Abstractions;
using Boilerplate.Shared.Exceptions;

using Mapster;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Business.CQRS.InvoiceDomain.Commands
{
    public class CreateInvoiceCommand : IRequest<Unit>
    {
        public string ExternalId { get; set; }

        public string CompanyIdentifier { get; set; }

        public DateTime ExternalCreatedAt { get; set; }

        [JsonIgnore]
        public bool ThrowException { get; set; }
    }

    internal class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.ExternalId).NotEmpty();
            RuleFor(x => x.CompanyIdentifier).NotEmpty();
            RuleFor(x => x.ExternalCreatedAt).NotEmpty();
        }
    }

    internal class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Unit>
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IEventBus _eventBus;

        public CreateInvoiceCommandHandler(IBaseRepository<Invoice> invoiceRepository, IEventBus eventBus)
        {
            _invoiceRepository = invoiceRepository;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            if (request.ThrowException)
            {
                throw new InvalidOperationException("This exception thrown to test resillience pipeline");
            }

            var invoiceExists = await _invoiceRepository.GetAll().AnyAsync(x => x.ExternalId == request.ExternalId && x.CompanyIdentifier == request.CompanyIdentifier, cancellationToken);

            if (invoiceExists)
            {
                throw new ConflictException(nameof(Invoice), $"{nameof(Invoice.ExternalId)}-{nameof(Invoice.CompanyIdentifier)}", $"{request.ExternalId}-{request.CompanyIdentifier}");
            }

            var invoice = new Invoice(request.ExternalId, request.CompanyIdentifier, request.ExternalCreatedAt);

            await _invoiceRepository.AddAsync(invoice, cancellationToken, saveChanges: true);

            var invoiceCreatedEvent = new InvoiceCreatedEvent();

            _eventBus.PublishAsync(invoice.Adapt(invoiceCreatedEvent), cancellationToken);

            return Unit.Value;
        }
    }
}
