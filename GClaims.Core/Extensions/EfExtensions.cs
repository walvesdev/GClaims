using Microsoft.EntityFrameworkCore;

namespace GClaims.Core.Extensions;

public static class EfExtensions
{
    public static Task<List<TSource>> ToListAsync<TSource>(
        this IQueryable<TSource> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source is not IAsyncEnumerable<TSource>
            ? Task.FromResult(source.ToList())
            : EntityFrameworkQueryableExtensions.ToListAsync(source);
    }
}