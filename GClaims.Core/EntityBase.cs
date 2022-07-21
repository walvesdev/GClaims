namespace GClaims.Core;

public abstract class EntityBase
{
    protected EntityBase()
    {
        Id = SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAsString);
    }

    public Guid Id { get; set; }
}

public abstract class EntityBase<TKey>
{
    public TKey Id { get; set; }
}