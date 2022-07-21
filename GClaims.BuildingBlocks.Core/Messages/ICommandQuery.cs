namespace GClaims.BuildingBlocks.Core.Messages;

public interface ICommandQuery
{
    public int CurrentPage { get; set; }

    public int SkipCount { get; set; }

    public int PageSize { get; set; }

    public int MaxResultCount { get; set; }

    public long TotalCount { get; set; }

    public string Sorting { get; set; }

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < SkipCount;
}