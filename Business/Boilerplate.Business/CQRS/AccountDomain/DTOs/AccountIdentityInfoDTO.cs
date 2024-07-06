using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Boilerplate.Business.CQRS.AccountDomain.DTOs
{
    public class AccountIdentityInfoDTO
    {
        public DateTime BirthDate { get; set; }

        public string? BirthPlace { get; set; }

        public string? IdentityCardSerialNo { get; set; }

        public string? Identity { get; set; }
    }

    internal class AccountIdentityInfoDTOValidator : AbstractValidator<AccountIdentityInfoDTO>
    {
        public AccountIdentityInfoDTOValidator()
        {
            RuleFor(x => x.BirthDate).NotEmpty();
        }
    }
}
