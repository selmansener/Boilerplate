using Boilerplate.Business.CQRS.AccountDomain.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.API.Areas.API.Controllers
{
    public class AccountController : APIBaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand createAccountCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(createAccountCommand, cancellationToken);

            return Ok();
        }
    }
}
