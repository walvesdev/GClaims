namespace GClaims.Core;

public class EntityBaseAudit : EntityBase
{
    public int? CreatorId { get; set; }

    public int? ModifierId { get; set; }

    public int? DeleterId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}

public class EntityBaseAudit<TKey> : EntityBase<TKey>
{
    public int? CreatorId { get; set; }

    public int? ModifierId { get; set; }

    public int? DeleterId { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}