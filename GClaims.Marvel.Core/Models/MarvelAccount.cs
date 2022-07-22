using Dapper.Contrib.Extensions;
using GClaims.Core;

namespace GClaims.Marvel.Core.Models;

[Table ("Account")]
public class MarvelAccount //: AggregateRoot<Guid>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    //public int RoleId { get; set; }
    
    public MarvelAccount()
    {
        //Id = SequentialGuidGenerator.NewSequentialGuid();
    }
}