using FluentValidation;
using GClaims.Marvel.Application.Accounts.Comands;

namespace GClaims.Marvel.Application.Validators.AccountValidators;

public class UpdateAccountValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountValidator()
    {
        RuleFor(c => c)
            .NotNull();

        RuleFor(c => c.Input)
            .NotNull();
    }
}