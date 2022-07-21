using GClaims.Core;

namespace GClaims.Marvel.Core.Models;

public class MarvelAccount : AggregateRoot<Guid>
{
    public MarvelAccount()
    {
        Id = SequentialGuidGenerator.NewSequentialGuid();
    }
}