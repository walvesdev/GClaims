#region

using GClaims.BuildingBlocks.Core.Messages;

#endregion

namespace GClaims.BuildingBlocks.Core.Common;

public interface IInputRequest<TRequest>
{
    public TRequest? Input { get; set; }
}

public interface IInputResponse<TResponse>
{
    public TResponse Response { get; set; }
}

public interface IInputQueryResponse<TResponse> : ICommandQuery
{
    public IEnumerable<TResponse> Items { get; set; }
}

public class InputQueryResponse<TResponse> : IInputQueryResponse<TResponse>
{
    protected InputQueryResponse(ICommandQuery query, ICollection<TResponse> list)
    {
        Items = query.ToPagedList(list);
        CurrentPage = query.CurrentPage;
        SkipCount = query.SkipCount;
        PageSize = query.PageSize;
        MaxResultCount = query.MaxResultCount;
        TotalCount = query.TotalCount;
        Sorting = query.Sorting;
    }

    public int CurrentPage { get; set; }

    public int SkipCount { get; set; }

    public int PageSize { get; set; }

    public int MaxResultCount { get; set; }

    public long TotalCount { get; set; }

    public string? Sorting { get; set; }

    public bool HasPrevious { get; set; }

    public bool HasNext { get; set; }

    public IEnumerable<TResponse> Items { get; set; }
}

public static class InputQueryExtensions
{
    public static ICollection<TResult> ToPagedList<TResult>(this ICommandQuery query, ICollection<TResult> list)
    {
        query.TotalCount = list.Count;
        var maxResultCount = query.MaxResultCount > list.Count ? list.Count : query.MaxResultCount;
        var skipCount = query.SkipCount > 0 ? query.SkipCount : (query.CurrentPage - 1) * query.PageSize;

        if (maxResultCount > 0)
        {
            list = list.Skip(0).Take(maxResultCount).ToList();
        }

        var pageSize = query.PageSize <= list.Count ? query.PageSize : list.Count;

        var result = query.CurrentPage <= 0
            ? list.Skip(0).Take(pageSize).ToList()
            : list.Skip(skipCount).Take(pageSize).ToList();
        try
        {
            if (!string.IsNullOrWhiteSpace(query.Sorting))
            {
                if (query.Sorting.ToLower().Contains("asc"))
                {
                    return result.OrderBy(_ => query.Sorting.Replace("asc", "").Trim()).ToList();
                }

                return query.Sorting.ToLower().Contains("desc")
                    ? result.OrderByDescending(_ => query.Sorting.Replace("desc", "").Trim()).ToList()
                    : result.OrderBy(_ => query.Sorting).ToList();
            }
        }
        catch
        {
            return result;
        }

        return result;
    }
}