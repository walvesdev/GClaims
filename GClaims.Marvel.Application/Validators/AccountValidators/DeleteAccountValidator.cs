using FluentValidation;
using GClaims.Marvel.Application.Accounts.Comands;

namespace GClaims.Marvel.Application.Validators.AccountValidators;

public class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountValidator()
    {
        RuleFor(c => c)
            .NotNull();

        RuleFor(c => c.Id)
            .GreaterThan(0);
    }
}