using Boilerplate.Domains.Base;

namespace Boilerplate.Domains.AccountDomain
{
    public class Account : BaseEntity
    {
        public Account(string email, string password, string iBAN, DateTime birthDate)
        {
            Email = email;
            Password = password;
            IBAN = iBAN;
            BirthDate = birthDate;
        }

        public string Email { get; private set; }

        public string Password { get; private set; }

        public string IBAN { get; private set; }

        public DateTime BirthDate { get; private set; }
    }
}
