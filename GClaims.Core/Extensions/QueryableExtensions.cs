using System.Linq.Expressions;
using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

/// <summary>
/// Alguns métodos de extensão úteis para <see cref="T:System.Linq.IQueryable`1" />.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Usado para paginação. Pode ser usado como uma alternativa ao encadeamento Skip(...).Take(...).
    /// </summary>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
    {
        Check.NotNull(query, "query");
        return query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Usado para paginação. Pode ser usado como uma alternativa ao encadeamento Skip(...).Take(...).
    /// </summary>
    public static TQueryable PageBy<T, TQueryable>(this TQueryable query, int skipCount, int maxResultCount)
        where TQueryable : IQueryable<T>
    {
        Check.NotNull(query, "query");
        return (TQueryable)query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Linq.IQueryable`1" /> por determinado predicado se determinada condição for verdadeira.
    /// </summary>
    /// <param name="query">Consultável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar a consulta</param>
    /// <returns>Consulta filtrada ou não filtrada com base em <paramref name="condition" /></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        Check.NotNull(query, "query");
        if (!condition)
        {
            return query;
        }

        return query.Where(predicate);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Linq.IQueryable`1" /> por determinado predicado se determinada condição for verdadeira.
    /// </summary>
    /// <param name="query">Consultável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar a consulta</param>
    /// <returns>Consulta filtrada ou não filtrada com base em <paramref name="condition" /></returns>
    public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition,
        Expression<Func<T, bool>> predicate) where TQueryable : IQueryable<T>
    {
        Check.NotNull(query, "query");
        if (!condition)
        {
            return query;
        }

        return (TQueryable)query.Where(predicate);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Linq.IQueryable`1" /> por determinado predicado se determinada condição for verdadeira.
    /// </summary>
    /// <param name="query">Consultável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar a consulta</param>
    /// <returns>Consulta filtrada ou não filtrada com base em <paramref name="condition" /></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        Check.NotNull(query, "query");
        if (!condition)
        {
            return query;
        }

        return query.Where(predicate);
    }

    /// <summary>
    /// Filtra um <see cref="T:System.Linq.IQueryable`1" /> por determinado predicado se determinada condição for verdadeira.
    /// </summary>
    /// <param name="query">Consultável para aplicar filtragem</param>
    /// <param name="condition">Um valor booleano</param>
    /// <param name="predicate">Predicado para filtrar a consulta</param>
    /// <returns>Consulta filtrada ou não filtrada com base em <paramref name="condition" /></returns>
    public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition,
        Expression<Func<T, int, bool>> predicate) where TQueryable : IQueryable<T>
    {
        Check.NotNull(query, "query");
        if (!condition)
        {
            return query;
        }

        return (TQueryable)query.Where(predicate);
    }
}