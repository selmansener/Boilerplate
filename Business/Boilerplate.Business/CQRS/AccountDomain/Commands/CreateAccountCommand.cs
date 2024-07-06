using FluentValidation;

using Boilerplate.Business.CQRS.AccountDomain.DTOs;
using Boilerplate.DataAccess.Repositories;
using Boilerplate.Domains.AccountDomain;

using Mapster;

using MediatR;

namespace Boilerplate.Business.CQRS.AccountDomain.Commands
{
    public class CreateAccountCommand : IRequest<Unit>
    {
        public required string Email { get; set; }

        public required string Password { get; set; }

        public required string IBAN { get; set; }

        public required AccountIdentityInfoDTO AccountIdentityInfo { get; set; }
    }

    internal class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.IBAN).NotEmpty();
            RuleFor(x => x.AccountIdentityInfo)
                .NotEmpty()
                .SetValidator(new AccountIdentityInfoDTOValidator());
        }
    }

    internal class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Unit>
    {
        private readonly IBaseRepository<Account> _accountRepository;

        public CreateAccountCommandHandler(IBaseRepository<Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account(request.Email, request.Password, request.IBAN, request.AccountIdentityInfo.BirthDate);

            await _accountRepository.AddAsync(account, cancellationToken, saveChanges: true);

            return Unit.Value;
        }
    }
}
