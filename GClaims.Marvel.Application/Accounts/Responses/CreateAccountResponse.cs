using GClaims.Marvel.Application.Accounts.Dtos;

namespace GClaims.Marvel.Application.Accounts.Responses;

public class CreateAccountResponse : MarvelAccountBaseDto
{
    public string Name { get; set; }
}