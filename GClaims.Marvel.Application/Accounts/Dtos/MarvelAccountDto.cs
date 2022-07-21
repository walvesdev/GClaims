#region

#endregion

namespace GClaims.Marvel.Application.Accounts.Dtos;

public class MarvelAccountDto : MarvelAccountBaseDto
{
    public string Name { get; set; }
}

public class MarvelAccountBaseDto 
{
    public int Id { get; set; }
}