using FluentValidation;
using GClaims.Marvel.Application.Accounts.Queries;

namespace GClaims.Marvel.Application.Validators.AccountValidators;

public class GetAllAccountValidator : AbstractValidator<GetAllAccountQuery>
{
    public static string SortingErroMsg => "Sorting inválido";

    public static string SkipCountErroMsg => "SkipCount inválido";

    public static string MaxResultCountErroMsg => "MaxResultCount informado";
}