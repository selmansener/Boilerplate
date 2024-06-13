using System.Threading;

using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;

using InvoiceFetcher.Business.CQRS.InvoiceDomain.Commands;
using InvoiceFetcher.Business.CQRS.InvoiceDomain.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace InvoiceFetcher.API.Areas.API.Controllers
{
    public class InvoiceController : APIBaseController
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var invoice = await _mediator.Send(new GetInvoiceById(id), cancellationToken);

            return Ok(invoice);
        }

        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand createInvoiceCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(createInvoiceCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("CreateInvoiceResillienceTest")]
        public async Task<IActionResult> CreateInvoiceResillienceTest([FromBody] CreateInvoiceCommand createInvoiceCommand, CancellationToken cancellationToken)
        {
            createInvoiceCommand.ThrowException = true;
            await _mediator.Send(createInvoiceCommand, cancellationToken);

            return Ok();
        }

        [HttpPost("UpdateInvoice/{id}")]
        public async Task<IActionResult> UpdateInvoice([FromRoute] int id, [FromBody] UpdateInvoiceCommand updateInvoiceCommand, CancellationToken cancellationToken)
        {
            updateInvoiceCommand.InvoiceId = id;
            await _mediator.Send(updateInvoiceCommand, cancellationToken);

            return Ok();
        }

        [HttpGet("Query")]
        [DynamicQuery]
        public async Task<IActionResult> Query([FromQuery] DynamicQueryOptions dqb, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new QueryInvoices(dqb), cancellationToken);

            return Ok(result);
        }
    }
}
