using FluentValidation;

using Boilerplate.Shared.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.API.Areas.Dev.Controllers
{
    public class ExceptionController : DevAPIBaseController
    {
        private readonly SomeDependency _someDependency;

        public ExceptionController(SomeDependency someDependency = null)
        {
            _someDependency = someDependency;
        }

        [HttpGet("TestOptionalDependency")]
        public IActionResult TestOptionalDependency()
        {
            return Ok();
        }

        [HttpGet("ThrowValidationException")]
        public IActionResult ThrowValidationException()
        {
            var someClass = new SomeClass();
            var validator = new SomeValidator();

            validator.ValidateAndThrow(someClass);

            return Ok();
        }

        [HttpGet("ThrowNotFoundException")]
        public IActionResult ThrowNotFoundException()
        {
            throw new NotFoundException("Account", "Id");
        }

        [HttpGet("ThrowConflictException")]
        public IActionResult ThrowConflictException()
        {
            throw new ConflictException("Account", "Id", "12");
        }

        [HttpGet("ThrowInvalidOperation")]
        public IActionResult ThrowInvalidOperation()
        {
            throw new InvalidOperationException("This is an exception message.");
        }
    }

    public class SomeClass
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class SomeValidator : AbstractValidator<SomeClass>
    {
        public SomeValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name.Length)
                .GreaterThan(5)
                .LessThan(500)
                .When(x => x.Name != null);
        }
    }

    public class SomeDependency
    {

    }
}
