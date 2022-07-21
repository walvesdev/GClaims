using FluentValidation;
using GClaims.Marvel.Application.Accounts.Comands;

namespace GClaims.Marvel.Application.Validators.AccountValidators;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(c => c)
            .NotNull()
            .WithMessage("CreateAccountCommand null");

        RuleFor(c => c.Request.Input)
            .NotNull()
            .WithMessage("Input null");
    }
}