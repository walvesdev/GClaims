#region

using GClaims.BuildingBlocks.Core.Messages;
using GClaims.Core.Extensions;

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
    public static ICollection<TResult> ToPagedList<TResult>(this ICommandQuery command, ICollection<TResult> list)
    {
        command.TotalCount = list.Count;
        var maxResultCount = command.MaxResultCount > list.Count ? list.Count : command.MaxResultCount;
        var skipCount = command.SkipCount > 0 ? command.SkipCount : (command.CurrentPage - 1) * command.PageSize;

        var query = list.AsQueryable();
        
        if (maxResultCount > 0)
        {
            query = list.Skip(0).Take(maxResultCount).AsQueryable();
        }

        var pageSize = command.PageSize <= list.Count ? command.PageSize : list.Count;

         query = command.CurrentPage <= 0
            ? list.Skip(0).Take(pageSize).AsQueryable()
            : list.Skip(skipCount).Take(pageSize).AsQueryable();
        try
        {
            if (!string.IsNullOrWhiteSpace(command.Sorting))
            {
                if (command.Sorting.ToLower().Contains("asc"))
                {
                    query = query.OrderByProperty(command.Sorting.Replace("asc","").Trim());
                    return query.ToList();
                }

                if (command.Sorting.ToLower().Contains("desc"))
                {
                    query = query.OrderByPropertyDesc(command.Sorting.Replace("desc","").Trim());
                    return query.ToList();
                }
                query = query.OrderByProperty(command.Sorting);
                return query.ToList();
            }
        }
        catch (Exception e)
        {
            return query.ToList();
        }

        return query.ToList();
    }
}