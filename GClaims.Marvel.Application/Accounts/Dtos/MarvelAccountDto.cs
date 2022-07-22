#region

#endregion

namespace GClaims.Marvel.Application.Accounts.Dtos;

public class MarvelAccountDto// : MarvelAccountBaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    //public int RoleId { get; set; }
}

public class MarvelAccountBaseDto 
{
    //public Guid Id { get; set; } = Guid.NewGuid();  
}