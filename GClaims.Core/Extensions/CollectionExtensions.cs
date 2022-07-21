using GClaims.Core.Helpers;

namespace GClaims.Core.Extensions;

/// <summary>
/// Métodos de extensão para coleções.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Verifica qualquer objeto de coleção fornecido é nulo ou não tem item.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this ICollection<T> source)
    {
        if (source != null)
        {
            return source.Count <= 0;
        }

        return true;
    }

    /// <summary>
    /// Adiciona um item à coleção se ainda não estiver na coleção.
    /// </summary>
    /// <param name="source">A coleção</param>
    /// <param name="item">Item para verificar e adicionar</param>
    /// <typeparam name="T">Tipo dos itens na coleção</typeparam>
    /// <returns>Retorna True se adicionado, retorna False se não for.</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        Check.NotNull(source, "source");
        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    /// <summary>
    /// Adiciona itens à coleção que ainda não estão na coleção.
    /// </summary>
    /// <param name="source">A coleção</param>
    /// <param name="items">Item para verificar e adicionar</param>
    /// <typeparam name="T">Tipo dos itens na coleção</typeparam>
    /// <returns>Retorna os itens adicionados.</returns>
    public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        Check.NotNull(source, "source");
        var addedItems = new List<T>();
        foreach (var item in items)
        {
            if (source.Contains(item))
            {
                continue;
            }

            source.Add(item);
            addedItems.Add(item);
        }

        return addedItems;
    }

    /// <summary>
    /// Adiciona um item à coleção se ainda não estiver na coleção com base no <paramref name="predicate" /> fornecido.
    /// </summary>
    /// <param name="source">A coleção</param>
    /// <param name="predicate">A condição para decidir se o item já está na coleção</param>
    /// <param name="itemFactory">Uma fábrica que retorna o item</param>
    /// <typeparam name="T">Tipo dos itens na coleção</typeparam>
    /// <returns>Retorna True se adicionado, retorna False se não for.</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
    {
        Check.NotNull(source, "source");
        Check.NotNull(predicate, "predicate");
        Check.NotNull(itemFactory, "itemFactory");
        if (source.Any(predicate))
        {
            return false;
        }

        source.Add(itemFactory());
        return true;
    }

    /// <summary>
    /// Remove todos os itens da coleção que satisfazem o <paramref name="predicate" /> fornecido.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens na coleção</typeparam>
    /// <param name="source">A coleção</param>
    /// <param name="predicate">A condição para remover os itens</param>
    /// <returns>Lista de itens removidos</returns>
    public static IList<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToList();
        foreach (var item in items)
        {
            source.Remove(item);
        }

        return items;
    }

    /// <summary>
    /// Remove todos os itens da coleção.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens na coleção</typeparam>
    /// <param name="source">A coleção</param>
    /// <param name="items">Itens a serem removidos da lista</param>
    public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Remove(item);
        }
    }
}